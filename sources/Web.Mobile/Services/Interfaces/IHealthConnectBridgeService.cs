namespace Web.Mobile.Services;

public interface IHealthConnectBridgeService
{
    bool IsSdkSupported();
    Task<bool> IsHealthConnectAvailableAsync();
    Task<bool> HasAllPermissionsAsync();
    Task<HealthConnectPermissionResult> RequestAllPermissionsAsync();
    Task<string> GetRequiredPermissionsJsonAsync();
    Task<string> GetGrantedPermissionsJsonAsync();
    Task<string> ReadAllRecordsAsJsonAsync(DateTimeOffset startUtc, DateTimeOffset endUtc);
}
