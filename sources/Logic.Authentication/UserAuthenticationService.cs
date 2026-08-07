using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;
using Logic.Authentication.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Models.Authentication;

namespace Logic.Authentication;

public class UserAuthenticationService(
    IApplicationUnitOfWork applicationUnitOfWork,
    DatabaseContext databaseContext,
    IPasswordHashService passwordHashService,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtOptions,
    IHttpContextAccessor httpContextAccessor)
    : LogicBase(applicationUnitOfWork), IUserAuthenticationService
{
    private readonly DatabaseContext _databaseContext = databaseContext;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<AuthResponseModel> RegisterAsync(RegisterRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        ValidateRegisterRequest(requestModel);

        var normalizedEmail = requestModel.Email.Trim().ToLowerInvariant();
        var normalizedUserName = requestModel.UserName.Trim();

        var userExists = await _databaseContext.Users.AnyAsync(
            user => user.Email.ToLower() == normalizedEmail || user.UserName.ToLower() == normalizedUserName.ToLower(),
            cancellationToken);

        if (userExists)
        {
            throw new InvalidOperationException("A user with the same username or email already exists.");
        }

        var role = await _databaseContext.Roles.FirstOrDefaultAsync(x => x.Name == UserRoleClaims.User, cancellationToken);
        if (role is null)
        {
            throw new InvalidOperationException("Required base role 'User' was not found. Ensure role seeding has run.");
        }

        var user = new UserEntity
        {
            Email = normalizedEmail,
            UserName = normalizedUserName,
            IsActive = true,
            Credentials = new UserCredentialsEntity
            {
                PasswordHash = _passwordHashService.HashPassword(requestModel.Password),
                FaildLoginAttemts = 0
            }
        };

        user.UserRoles.Add(new UserRoleEntity { Role = role });

        await ApplicationUnitOfWork.Users.AddAsync(user, cancellationToken);

        var nowUtc = DateTime.UtcNow;
        var refreshToken = CreateRefreshTokenEntity(user.Credentials!, nowUtc, GetRequestIp());
        user.Credentials!.RefreshTokens.Add(refreshToken.Entity);

        await ApplicationUnitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenService.CreateAccessToken(user, nowUtc);

        return new AuthResponseModel
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
            RefreshToken = refreshToken.RawToken
        };
    }

    public async Task<AuthResponseModel> LoginAsync(LoginRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(requestModel.UserNameOrEmail) || string.IsNullOrWhiteSpace(requestModel.Password))
        {
            throw new ArgumentException("Username/email and password are required.");
        }

        var normalizedIdentity = requestModel.UserNameOrEmail.Trim().ToLowerInvariant();

        var user = await _databaseContext.Users
            .Include(x => x.Credentials)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .Include(x => x.UserModulePermissions)
                .ThenInclude(x => x.ModulePermission)
            .FirstOrDefaultAsync(
                x => x.Email.ToLower() == normalizedIdentity || x.UserName.ToLower() == normalizedIdentity,
                cancellationToken);

        if (user is null || user.Credentials is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var isValidPassword = _passwordHashService.VerifyPassword(requestModel.Password, user.Credentials.PasswordHash);
        if (!isValidPassword)
        {
            user.Credentials.FaildLoginAttemts += 1;
            await ApplicationUnitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (user.Credentials.FaildLoginAttemts > 0)
        {
            user.Credentials.FaildLoginAttemts = 0;
        }

        var nowUtc = DateTime.UtcNow;
        var refreshToken = CreateRefreshTokenEntity(user.Credentials, nowUtc, GetRequestIp());
        await _databaseContext.RefreshTokens.AddAsync(refreshToken.Entity, cancellationToken);

        await ApplicationUnitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenService.CreateAccessToken(user, nowUtc);

        return new AuthResponseModel
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
            RefreshToken = refreshToken.RawToken
        };
    }

    public async Task<AuthResponseModel> RefreshAsync(RefreshTokenRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(requestModel.RefreshToken))
        {
            throw new ArgumentException("Refresh token is required.");
        }

        var tokenHash = _jwtTokenService.ComputeTokenHash(requestModel.RefreshToken);

        var refreshToken = await _databaseContext.RefreshTokens
            .Include(x => x.UserCredentials)
                .ThenInclude(x => x.User)
                    .ThenInclude(x => x.UserRoles)
                        .ThenInclude(x => x.Role)
            .Include(x => x.UserCredentials)
                .ThenInclude(x => x.User)
                    .ThenInclude(x => x.UserModulePermissions)
                        .ThenInclude(x => x.ModulePermission)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (refreshToken is null || refreshToken.RevokedAtUtc.HasValue || refreshToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var user = refreshToken.UserCredentials.User;
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User is not active.");
        }

        var nowUtc = DateTime.UtcNow;
        var rotatedToken = CreateRefreshTokenEntity(refreshToken.UserCredentials, nowUtc, GetRequestIp());

        refreshToken.RevokedAtUtc = nowUtc;
        refreshToken.RevokedByIp = rotatedToken.Entity.CreatedByIp;
        refreshToken.ReplacedByTokenHash = rotatedToken.Entity.TokenHash;

        await _databaseContext.RefreshTokens.AddAsync(rotatedToken.Entity, cancellationToken);
        await ApplicationUnitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenService.CreateAccessToken(user, nowUtc);

        return new AuthResponseModel
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
            RefreshToken = rotatedToken.RawToken
        };
    }

    public async Task<bool> RevokeAsync(RevokeTokenRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(requestModel.RefreshToken))
        {
            throw new ArgumentException("Refresh token is required.");
        }

        var tokenHash = _jwtTokenService.ComputeTokenHash(requestModel.RefreshToken);
        var refreshToken = await _databaseContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (refreshToken is null || refreshToken.RevokedAtUtc.HasValue)
        {
            return false;
        }

        refreshToken.RevokedAtUtc = DateTime.UtcNow;
        refreshToken.RevokedByIp = GetRequestIp();

        await ApplicationUnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ValidateRegisterRequest(RegisterRequestModel requestModel)
    {
        if (string.IsNullOrWhiteSpace(requestModel.UserName))
        {
            throw new ArgumentException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(requestModel.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(requestModel.Password))
        {
            throw new ArgumentException("Password is required.");
        }
    }

    private string GetRequestIp()
    {
        return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private RefreshTokenCreationResult CreateRefreshTokenEntity(UserCredentialsEntity credentials, DateTime nowUtc, string requestIp)
    {
        var rawToken = _jwtTokenService.CreateRefreshToken();
        var tokenHash = _jwtTokenService.ComputeTokenHash(rawToken);

        var entity = new RefreshTokenEntity
        {
            UserCredentials = credentials,
            TokenHash = tokenHash,
            ExpiresAtUtc = nowUtc.AddDays(_jwtOptions.RefreshTokenDays),
            CreatedByIp = requestIp
        };

        return new RefreshTokenCreationResult(rawToken, entity);
    }

    private sealed record RefreshTokenCreationResult(string RawToken, RefreshTokenEntity Entity);
}
