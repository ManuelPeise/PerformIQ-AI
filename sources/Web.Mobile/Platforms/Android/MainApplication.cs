using Android.App;
using Android.Runtime;

namespace Web.Mobile
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public static IServiceProvider? CurrentServices { get; private set; }

        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            var mauiApp = MauiProgram.CreateMauiApp();
            CurrentServices = mauiApp.Services;
            return mauiApp;
        }
    }
}
