namespace Web.Mobile.Services;

public sealed class HealthConnectPermissionResult
{
    public bool IsGranted { get; init; }
    public string PermissionsJson { get; init; } = "[]";
}
