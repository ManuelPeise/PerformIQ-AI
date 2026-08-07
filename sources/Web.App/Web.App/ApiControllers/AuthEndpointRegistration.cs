using Logic.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Shared.Models.Authentication;

namespace Web.App.ApiControllers;

internal static class AuthEndpointRegistration
{
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
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await authenticationService.RegisterAsync(requestModel, cancellationToken);
            return Results.Ok(response);
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
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await authenticationService.LoginAsync(requestModel, cancellationToken);
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

    private static async Task<IResult> RevokeAsync(
        RevokeTokenRequestModel requestModel,
        IUserAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var revoked = await authenticationService.RevokeAsync(requestModel, cancellationToken);
            return revoked ? Results.Ok() : Results.NotFound();
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }
}
