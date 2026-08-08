using Logic.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Shared.Models.Authentication;

namespace Web.App.ApiControllers;

internal static class AuthEndpointRegistration
{
    private const string RefreshTokenCookieName = "performiq.refresh-token";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", RegisterAsync).AllowAnonymous();
        group.MapPost("/login", LoginAsync).AllowAnonymous();
        group.MapPost("/refresh", RefreshAsync).AllowAnonymous();
        group.MapPost("/revoke", RevokeAsync).AllowAnonymous();
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequestModel requestModel,
        IUserAuthenticationService authenticationService,
        IOptions<JwtOptions> jwtOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await authenticationService.RegisterAsync(requestModel, cancellationToken);
            SetRefreshTokenCookie(httpContext, response.RefreshToken, jwtOptions.Value.RefreshTokenDays);
            return Results.Ok(ToSessionResponse(response));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static async Task<IResult> LoginAsync(
        LoginRequestModel requestModel,
        IUserAuthenticationService authenticationService,
        IOptions<JwtOptions> jwtOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await authenticationService.LoginAsync(requestModel, cancellationToken);
            SetRefreshTokenCookie(httpContext, response.RefreshToken, jwtOptions.Value.RefreshTokenDays);
            return Results.Ok(ToSessionResponse(response));
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

    private static async Task<IResult> RefreshAsync(
        RefreshTokenRequestModel? requestModel,
        IUserAuthenticationService authenticationService,
        IOptions<JwtOptions> jwtOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = ResolveRefreshToken(requestModel?.RefreshToken, httpContext);
            var response = await authenticationService.RefreshAsync(
                new RefreshTokenRequestModel { RefreshToken = refreshToken },
                cancellationToken);

            SetRefreshTokenCookie(httpContext, response.RefreshToken, jwtOptions.Value.RefreshTokenDays);
            return Results.Ok(ToSessionResponse(response));
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

    private static async Task<IResult> RevokeAsync(
        RevokeTokenRequestModel? requestModel,
        IUserAuthenticationService authenticationService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = ResolveRefreshToken(requestModel?.RefreshToken, httpContext);
            var revoked = await authenticationService.RevokeAsync(
                new RevokeTokenRequestModel { RefreshToken = refreshToken },
                cancellationToken);

            if (revoked)
            {
                ClearRefreshTokenCookie(httpContext);
                return Results.Ok();
            }

            return Results.NotFound();
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static AuthSessionResponseModel ToSessionResponse(AuthResponseModel response)
    {
        return new AuthSessionResponseModel
        {
            AccessToken = response.AccessToken,
            AccessTokenExpiresAtUtc = response.AccessTokenExpiresAtUtc
        };
    }

    private static string ResolveRefreshToken(string? requestToken, HttpContext httpContext)
    {
        var requestValue = requestToken?.Trim();
        if (!string.IsNullOrWhiteSpace(requestValue))
        {
            return requestValue;
        }

        if (httpContext.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var cookieToken)
            && !string.IsNullOrWhiteSpace(cookieToken))
        {
            return cookieToken;
        }

        throw new ArgumentException("Refresh token is required.");
    }

    private static void SetRefreshTokenCookie(HttpContext httpContext, string refreshToken, int refreshTokenDays)
    {
        var isHttps = httpContext.Request.IsHttps;

        httpContext.Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenDays)
            });
    }

    private static void ClearRefreshTokenCookie(HttpContext httpContext)
    {
        var isHttps = httpContext.Request.IsHttps;

        httpContext.Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
    }
}
