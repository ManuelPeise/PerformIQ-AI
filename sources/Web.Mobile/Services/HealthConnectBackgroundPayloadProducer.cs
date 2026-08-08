using System.Text.Json;
using Shared.Models.Mobile;

namespace Web.Mobile.Services;

public sealed class HealthConnectBackgroundPayloadProducer : IBackgroundPayloadProducer
{
    private readonly IHealthConnectBridgeService _healthConnectBridgeService;

    public HealthConnectBackgroundPayloadProducer(IHealthConnectBridgeService healthConnectBridgeService)
    {
        _healthConnectBridgeService = healthConnectBridgeService;
    }

    public string Name => "health-connect";

    public async Task<BackgroundSyncPayloadItemModel?> ProduceAsync(CancellationToken cancellationToken = default)
    {
        var supported = _healthConnectBridgeService.IsSdkSupported();
        var available = await _healthConnectBridgeService.IsHealthConnectAvailableAsync();
        var hasPermissions = available && await _healthConnectBridgeService.HasAllPermissionsAsync();

        var payload = new Dictionary<string, object?>
        {
            ["isSdkSupported"] = supported,
            ["isHealthConnectAvailable"] = available,
            ["hasAllPermissions"] = hasPermissions
        };

        if (hasPermissions)
        {
            var endUtc = DateTimeOffset.UtcNow;
            var startUtc = endUtc.AddHours(-24);
            var json = await _healthConnectBridgeService.ReadAllRecordsAsJsonAsync(startUtc, endUtc);
            payload["records"] = JsonSerializer.Deserialize<JsonElement>(json);
        }

        return new BackgroundSyncPayloadItemModel
        {
            Producer = Name,
            PayloadType = "health-connect-sync",
            CollectedAtUtc = DateTime.UtcNow,
            Payload = JsonSerializer.SerializeToElement(payload)
        };
    }
}
