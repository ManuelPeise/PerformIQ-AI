using Microsoft.Maui.ApplicationModel;
using System.Text.Json;
using Web.Mobile;

namespace Web.Mobile.Services;

public sealed class HealthConnectBridgeService : IHealthConnectBridgeService
{
    private readonly SemaphoreSlim _permissionRequestLock = new(1, 1);

    public bool IsSdkSupported()
    {
#if ANDROID
        return Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.IsSdkSupported();
#else
        return false;
#endif
    }

    public Task<bool> IsHealthConnectAvailableAsync()
    {
#if ANDROID
        return Task.Run(() => Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.IsHealthConnectAvailable(
            Microsoft.Maui.ApplicationModel.Platform.AppContext));
#else
        return Task.FromResult(false);
#endif
    }

    public Task<bool> HasAllPermissionsAsync()
    {
#if ANDROID
        return Task.Run(() => Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.HasAllPermissions(
            Microsoft.Maui.ApplicationModel.Platform.AppContext));
#else
        return Task.FromResult(false);
#endif
    }

    public async Task<HealthConnectPermissionResult> RequestAllPermissionsAsync()
    {
#if ANDROID
        await _permissionRequestLock.WaitAsync();
        try
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity as MainActivity;
            if (activity is null)
            {
                return BuildResult(false, "[]");
            }

            var requiredPermissionsJson = Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.GetRequiredPermissionsJson();
            var requiredPermissions = JsonSerializer.Deserialize<string[]>(requiredPermissionsJson) ?? Array.Empty<string>();
            if (requiredPermissions.Length == 0)
            {
                return BuildResult(false, "[]");
            }

            var completionSource = new TaskCompletionSource<HealthConnectPermissionResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            void PermissionResultHandler(HealthConnectPermissionResult result) => completionSource.TrySetResult(result);

            MainActivity.HealthConnectPermissionResultReceived += PermissionResultHandler;
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AndroidX.Core.App.ActivityCompat.RequestPermissions(
                        activity,
                        requiredPermissions,
                        Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.PermissionRequestCode);
                });

                var completedTask = await Task.WhenAny(completionSource.Task, Task.Delay(TimeSpan.FromMinutes(2)));
                if (completedTask == completionSource.Task)
                {
                    return await completionSource.Task;
                }

                return BuildResult(false, "[]");
            }
            finally
            {
                MainActivity.HealthConnectPermissionResultReceived -= PermissionResultHandler;
            }
        }
        finally
        {
            _permissionRequestLock.Release();
        }
#else
        return BuildResult(false, "[]");
#endif
    }

    public Task<string> GetRequiredPermissionsJsonAsync()
    {
#if ANDROID
        var json = Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.GetRequiredPermissionsJson();
        return Task.FromResult(json);
#else
        return Task.FromResult("[]");
#endif
    }

    public Task<string> GetGrantedPermissionsJsonAsync()
    {
#if ANDROID
        return Task.Run(() => Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.GetGrantedPermissionsJson(
            Microsoft.Maui.ApplicationModel.Platform.AppContext));
#else
        return Task.FromResult("[]");
#endif
    }

    public Task<string> ReadAllRecordsAsJsonAsync(DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
#if ANDROID
        return Task.Run(() => Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.ReadAllRecordsAsJson(
            Microsoft.Maui.ApplicationModel.Platform.AppContext,
            startUtc.ToUnixTimeMilliseconds(),
            endUtc.ToUnixTimeMilliseconds()));
#else
        return Task.FromResult("{}");
#endif
    }

    private static HealthConnectPermissionResult BuildResult(bool isGranted, string permissionsJson)
    {
        return new HealthConnectPermissionResult
        {
            IsGranted = isGranted,
            PermissionsJson = permissionsJson
        };
    }
}
