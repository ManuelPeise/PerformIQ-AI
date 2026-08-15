using Logic.Authentication.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Models.Authentication;

namespace Web.Api.Services.ApiControllers.Authentication
{
    internal static class AuthenticationEndpointMapper
    {
        private const string RefreshTokenCookieName = "refresh_token";

        internal static void MapAuthEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            var group = endpointRouteBuilder.MapGroup("/api/auth").WithTags("Authentication");

            group.MapPost("/login", LoginAsync).AllowAnonymous();
            group.MapPost("/register", RegisterAsync).AllowAnonymous();
            group.MapGet("/getcurrentuser", GetCurrentUser).RequireAuthorization("is-authenticated");

        }

        private static async Task<AuthResponseModel> LoginAsync(
        LoginRequestModel requestModel,
        IUserAuthenticationService authenticationService,
        IOptions<JwtOptions> jwtOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
        {

            var response = await authenticationService.LoginAsync(requestModel, cancellationToken);
            SetRefreshTokenCookie(httpContext, response.RefreshToken, jwtOptions.Value.RefreshTokenDays);
            return response;

        }

        private static async Task<bool> RegisterAsync(
        RegisterRequestModel requestModel,
        IUserAuthenticationService authenticationService,
        IOptions<JwtOptions> jwtOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
        {

            var response = await authenticationService.RegisterAsync(requestModel, cancellationToken);

            return response;

        }

        private static async Task<CurrentUserModel?> GetCurrentUser(
           ICurrentUserService currentUserService)
        {
            var response = await currentUserService.GetCurrentUser();
            return response;
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
    }
}
