using Android.App;
using Android.Content;
using Android.OS;

namespace Web.Mobile;

[Activity(
    Name = "com.companyname.web.mobile.PermissionsRationaleActivity",
    Exported = true,
    NoHistory = true,
    Theme = "@style/Theme.MaterialComponents.DayNight.NoActionBar")]
[IntentFilter(
    new[] { "androidx.health.ACTION_SHOW_PERMISSIONS_RATIONALE" },
    Categories = new[] { Intent.CategoryDefault })]
public sealed class PermissionsRationaleActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Finish();
    }
}
