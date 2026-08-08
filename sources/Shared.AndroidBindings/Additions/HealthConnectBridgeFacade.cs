using Android.Content;
using Com.Performiq.Healthconnectbridge;

namespace Shared.AndroidBindings.Additions;

public static class HealthConnectBridgeFacade
{
    public const int PermissionRequestCode = HealthConnectBridge.PermissionRequestCode;

    public static bool IsSdkSupported()
    {
        return HealthConnectBridge.IsSdkSupported;
    }

    public static bool IsHealthConnectAvailable(Context context)
    {
        return HealthConnectBridge.IsHealthConnectAvailable(context);
    }

    public static string GetRequiredPermissionsJson()
    {
        return HealthConnectBridge.RequiredPermissionsJson;
    }

    public static Intent? CreatePermissionRequestIntent(Context context)
    {
        return HealthConnectBridge.CreatePermissionRequestIntent(context);
    }

    public static string ParsePermissionResultJson(int resultCode, Intent? data)
    {
        return HealthConnectBridge.ParsePermissionResultJson(resultCode, data);
    }

    public static bool HasAllPermissions(Context context)
    {
        return HealthConnectBridge.HasAllPermissions(context);
    }

    public static string GetGrantedPermissionsJson(Context context)
    {
        return HealthConnectBridge.GetGrantedPermissionsJson(context);
    }

    public static string ReadAllRecordsAsJson(Context context, long startEpochMsUtc, long endEpochMsUtc)
    {
        return HealthConnectBridge.ReadAllRecordsAsJson(context, startEpochMsUtc, endEpochMsUtc);
    }
}
