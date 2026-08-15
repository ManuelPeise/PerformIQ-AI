using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;
using Logic.Authentication.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models.Authentication;

namespace Logic.Authentication;

public class UserAuthenticationService(
    ILogger<UserAuthenticationService> logger,
    IApplicationUnitOfWork applicationUnitOfWork,
    IPasswordHashService passwordHashService,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtOptions,
    IHttpContextAccessor httpContextAccessor)
    : LogicBase(applicationUnitOfWork, httpContextAccessor), IUserAuthenticationService
{
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<UserAuthenticationService> _logger = logger;
    public async Task<bool> RegisterAsync(RegisterRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRegisterRequest(requestModel);

            var normalizedEmail = requestModel.Email.Trim().ToLowerInvariant();
            var normalizedUserName = requestModel.UserName.Trim();

            var existingUsers = await ApplicationUnitOfWork.Users.GetAsync(
                new DbQueryOptions<UserEntity>
                {
                    WhereExpression = user =>
                        user.Email.ToLower() == normalizedEmail || user.UserName.ToLower() == normalizedUserName.ToLower()
                },
                cancellationToken);
            var userExists = existingUsers.Count > 0;

            if (userExists)
            {
                throw new InvalidOperationException("A user with the same username or email already exists.");
            }

            var roles = await ApplicationUnitOfWork.Roles.GetAsync(
                new DbQueryOptions<RoleEntity> { WhereExpression = x => x.Name == UserRoleClaims.User },
                cancellationToken);

            var role = roles.FirstOrDefault();

            if (role is null)
            {
                throw new InvalidOperationException("Required base role 'User' was not found. Ensure role seeding has run.");
            }

            var user = new UserEntity
            {
                Email = normalizedEmail,
                UserName = normalizedUserName,
                IsActive = true,
                Profile = new UserProfileEntity
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    DateOfBirth = null,
                    Address = new UserAddressEntity
                    {
                        Street = string.Empty,
                        HouseNumber = string.Empty,
                        PostalCode = string.Empty,
                        City = string.Empty,
                        StateOrProvince = string.Empty,
                        CountryCode = string.Empty
                    }
                },
                Credentials = new UserCredentialsEntity
                {
                    PasswordHash = _passwordHashService.HashPassword(requestModel.Password),
                    FaildLoginAttemts = 0
                }
            };

            user.UserRoles.Add(new UserRoleEntity { Role = role });

            var grantedModulesEntities = await ApplicationUnitOfWork.Modules.GetAsync(
                new DbQueryOptions<ModuleEntity>
                {
                    WhereExpression = x => x.IsDefaultModule,
                },
                cancellationToken);

            var userModulePermissions = grantedModulesEntities.Select(module => new ModulePermissionEntity
            {
                ModuleId = module.Id,
                CanView = true,
                CanCreate = true,
                CanEdit = true,
                CanDelete = false,
            }).ToList();

            user.ModulePermissions.AddRange(userModulePermissions);

            await ApplicationUnitOfWork.Users.AddAsync(user, cancellationToken);

            await ApplicationUnitOfWork.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred during user registration.");

            return await Task.FromResult(false);
        }
    }

    public async Task<AuthResponseModel?> LoginAsync(LoginRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.UserNameOrEmail) || string.IsNullOrWhiteSpace(requestModel.Password))
            {
                throw new ArgumentException("Username/email and password are required.");
            }

            var normalizedIdentity = requestModel.UserNameOrEmail.Trim().ToLowerInvariant();

            var users = await ApplicationUnitOfWork.Users.GetAsync(
                new DbQueryOptions<UserEntity>
                {
                    Includes = { x => x.Credentials },
                    WhereExpression = x => x.Email.ToLower() == normalizedIdentity || x.UserName.ToLower() == normalizedIdentity
                },
                cancellationToken);
            var user = users.FirstOrDefault();

            if (user is null || user.Credentials is null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            await PopulateUserClaimsDataAsync(user, cancellationToken);

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
            await ApplicationUnitOfWork.RefreshTokens.AddAsync(refreshToken.Entity, cancellationToken);

            await ApplicationUnitOfWork.SaveChangesAsync(cancellationToken);

            var accessToken = _jwtTokenService.CreateAccessToken(user, nowUtc);

            return new AuthResponseModel
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
                RefreshToken = refreshToken.RawToken
            };

        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred during user login.");

            return null;
        }
    }

    public async Task<AuthResponseModel> RefreshAsync(RefreshTokenRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(requestModel.RefreshToken))
        {
            throw new ArgumentException("Refresh token is required.");
        }

        var tokenHash = _jwtTokenService.ComputeTokenHash(requestModel.RefreshToken);

        var refreshTokens = await ApplicationUnitOfWork.RefreshTokens.GetAsync(
            new DbQueryOptions<RefreshTokenEntity>
            {
                Includes = { x => x.UserCredentials },
                WhereExpression = x => x.TokenHash == tokenHash
            },
            cancellationToken);
        var refreshToken = refreshTokens.FirstOrDefault();

        if (refreshToken is null || refreshToken.RevokedAtUtc.HasValue || refreshToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var users = await ApplicationUnitOfWork.Users.GetAsync(
            new DbQueryOptions<UserEntity>
            {
                Includes = { x => x.Credentials },
                WhereExpression = x => x.Id == refreshToken.UserCredentials.UserId
            },
            cancellationToken);
        var user = users.FirstOrDefault();
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        await PopulateUserClaimsDataAsync(user, cancellationToken);

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User is not active.");
        }

        var nowUtc = DateTime.UtcNow;
        var rotatedToken = CreateRefreshTokenEntity(refreshToken.UserCredentials, nowUtc, GetRequestIp());

        refreshToken.RevokedAtUtc = nowUtc;
        refreshToken.RevokedByIp = rotatedToken.Entity.CreatedByIp;
        refreshToken.ReplacedByTokenHash = rotatedToken.Entity.TokenHash;

        await ApplicationUnitOfWork.RefreshTokens.AddAsync(rotatedToken.Entity, cancellationToken);
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
        var refreshTokens = await ApplicationUnitOfWork.RefreshTokens.GetAsync(
            new DbQueryOptions<RefreshTokenEntity>
            {
                WhereExpression = x => x.TokenHash == tokenHash
            },
            cancellationToken);
        var refreshToken = refreshTokens.FirstOrDefault();

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

    private async Task PopulateUserClaimsDataAsync(UserEntity user, CancellationToken cancellationToken)
    {
        var userRoles = await ApplicationUnitOfWork.UserRoles.GetAsync(
            new DbQueryOptions<UserRoleEntity>
            {
                Includes = { x => x.Role },
                WhereExpression = x => x.UserId == user.Id
            },
            cancellationToken);

        var userModulePermissions = await ApplicationUnitOfWork.ModulePermissions.GetAsync(
            new DbQueryOptions<ModulePermissionEntity>
            {
                Includes = { x => x.Module },
                WhereExpression = x => x.UserId == user.Id
            },
            cancellationToken);

        user.UserRoles = userRoles.ToList();
        user.ModulePermissions = userModulePermissions.ToList();
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
