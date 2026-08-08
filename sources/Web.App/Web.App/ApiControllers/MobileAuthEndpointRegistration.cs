using Logic.Authentication.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Database;
using Shared.Models.Authentication;

namespace Web.App.ApiControllers;

internal static class MobileAuthEndpointRegistration
{
    public static void MapMobileAuthEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/api/mobile/auth").WithTags("Mobile Authentication");
        group.MapPost("/login", LoginAsync).AllowAnonymous();
        group.MapPost("/refresh", RefreshAsync).AllowAnonymous();
    }

    private static async Task<IResult> LoginAsync(
        LoginRequestModel requestModel,
        IUserAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await authenticationService.LoginAsync(requestModel, cancellationToken);
            var blockedRole = GetBlockedRole(response.AccessToken);
            if (blockedRole is not null)
            {
                await authenticationService.RevokeAsync(
                    new RevokeTokenRequestModel { RefreshToken = response.RefreshToken },
                    cancellationToken);

                return Results.Json(
                    new { messageKey = MapBlockedRoleToMessageKey(blockedRole) },
                    statusCode: StatusCodes.Status403Forbidden);
            }

            return Results.Ok(response);
        }
        catch (ArgumentException)
        {
            return Results.BadRequest(new { messageKey = "auth.mobile.invalid_request" });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Json(new { messageKey = "auth.mobile.invalid_credentials" }, statusCode: StatusCodes.Status401Unauthorized);
        }
    }

    private static async Task<IResult> RefreshAsync(
        RefreshTokenRequestModel requestModel,
        IUserAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await authenticationService.RefreshAsync(requestModel, cancellationToken);
            return Results.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static string? GetBlockedRole(string accessToken)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var roles = token.Claims
            .Where(claim => claim.Type == ClaimTypes.Role || claim.Type == "role")
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (roles.Contains(UserRoleClaims.SystemAdmin))
        {
            return UserRoleClaims.SystemAdmin;
        }

        if (roles.Contains(UserRoleClaims.Admin))
        {
            return UserRoleClaims.Admin;
        }

        if (roles.Contains(UserRoleClaims.Guest))
        {
            return UserRoleClaims.Guest;
        }

        return null;
    }

    private static string MapBlockedRoleToMessageKey(string role)
    {
        if (string.Equals(role, UserRoleClaims.SystemAdmin, StringComparison.OrdinalIgnoreCase))
        {
            return "auth.mobile.role_not_allowed_system_admin";
        }

        if (string.Equals(role, UserRoleClaims.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return "auth.mobile.role_not_allowed_admin";
        }

        return "auth.mobile.role_not_allowed_guest";
    }
}
