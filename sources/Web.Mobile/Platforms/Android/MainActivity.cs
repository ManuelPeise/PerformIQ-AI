using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using System.Text.Json;
using Web.Mobile.Services;

namespace Web.Mobile
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static event Action<HealthConnectPermissionResult>? HealthConnectPermissionResultReceived;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode != Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.PermissionRequestCode)
            {
                return;
            }

            var permissionsJson = Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.ParsePermissionResultJson((int)resultCode, data);
            var hasAllPermissions = Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.HasAllPermissions(this);
            var result = new HealthConnectPermissionResult
            {
                IsGranted = hasAllPermissions,
                PermissionsJson = permissionsJson
            };
            HealthConnectPermissionResultReceived?.Invoke(result);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode != Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.PermissionRequestCode)
            {
                return;
            }

            var grantedPermissions = permissions
                .Where((_, index) => index < grantResults.Length && grantResults[index] == Permission.Granted)
                .ToArray();

            var hasAllPermissions = Shared.AndroidBindings.Additions.HealthConnectBridgeFacade.HasAllPermissions(this);
            var result = new HealthConnectPermissionResult
            {
                IsGranted = hasAllPermissions,
                PermissionsJson = JsonSerializer.Serialize(grantedPermissions)
            };
            HealthConnectPermissionResultReceived?.Invoke(result);
        }
    }
}
