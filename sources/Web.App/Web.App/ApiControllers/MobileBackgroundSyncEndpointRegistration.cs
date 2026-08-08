using Shared.Models.Mobile;

namespace Web.App.ApiControllers;

internal static class MobileBackgroundSyncEndpointRegistration
{
    public static void MapMobileBackgroundSyncEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/api/mobile/background-sync")
            .WithTags("Mobile Background Sync")
            .RequireAuthorization();

        group.MapGet("/config", GetConfigAsync);
        group.MapPost("/data", ReceiveDataAsync);
    }

    private static IResult GetConfigAsync()
    {
        var response = new BackgroundSyncConfigResponseModel
        {
            IsActive = true,
            IntervalMinutes = 30
        };

        return Results.Ok(response);
    }

    private static IResult ReceiveDataAsync(
        BackgroundSyncDataRequestModel requestModel,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("MobileBackgroundSync");
        logger.LogInformation(
            "Received mobile background payload with {ItemCount} item(s) at {GeneratedAtUtc}.",
            requestModel.Items.Count,
            requestModel.GeneratedAtUtc);

        return Results.Ok();
    }
}
