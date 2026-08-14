using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;
using Logic.Authentication.Interfaces;
using Logic.Shared.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Models.Authentication;

namespace Logic.Authentication;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions) : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public AccessTokenResult CreateAccessToken(UserEntity user, DateTime nowUtc)
    {
        var expiresAtUtc = nowUtc.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in user.UserRoles.Select(x => x.Role.Name).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var moduleScope in user.ModulePermissions.SelectMany(scope => BuildModuleScopes(scope)).Distinct())        
        {
            claims.Add(new Claim(AuthClaimTypes.ModuleScope, moduleScope));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AccessTokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public string CreateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Base64UrlEncoder.Encode(randomBytes);
    }

    public string ComputeTokenHash(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToHexString(hashBytes);
    }

    private static IEnumerable<string> BuildModuleScopes(ModulePermissionEntity modulePermission)
    {
        var module = modulePermission.Module;

        if (modulePermission.CanView)
        {
            yield return $"{module}:canview";
        }

        if (modulePermission.CanEdit)
        {
            yield return $"{module}:canedit";
        }

        if (modulePermission.CanCreate)
        {
            yield return $"{module}:cancreate";
        }

        if (modulePermission.CanDelete)
        {
            yield return $"{module}:candelete";
        }
    }
}
