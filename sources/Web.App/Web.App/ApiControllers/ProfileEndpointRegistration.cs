using System.Security.Claims;
using Logic.Modules.Interfaces;
using Shared.Models.Profile;

namespace Web.App.ApiControllers;

internal static class ProfileEndpointRegistration
{
    public static void MapProfileEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/api/profile")
            .WithTags("Profile")
            .RequireAuthorization();

        group.MapGet("/", GetProfileAsync);
        group.MapPut("/", UpdateProfileAsync);
    }

    private static async Task<IResult> GetProfileAsync(
        ClaimsPrincipal user,
        IProfileServiceModule profileServiceModule,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId(user);
            var profile = await profileServiceModule.GetByUserIdAsync(userId, cancellationToken);
            return Results.Ok(profile);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    private static async Task<IResult> UpdateProfileAsync(
        UpdateUserProfileRequestModel requestModel,
        ClaimsPrincipal user,
        IProfileServiceModule profileServiceModule,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId(user);
            var profile = await profileServiceModule.UpsertByUserIdAsync(userId, requestModel, cancellationToken);
            return Results.Ok(profile);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    private static int GetUserId(ClaimsPrincipal user)
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdValue) || !int.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException("Authenticated user id claim is missing.");
        }

        return userId;
    }
}
