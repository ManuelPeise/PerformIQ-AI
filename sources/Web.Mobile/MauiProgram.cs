using Microsoft.Extensions.Logging;
using Web.Mobile.Pages;
using Web.Mobile.Pages.ViewModels;
using Web.Mobile.Services;

namespace Web.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
            builder.Services.AddSingleton<IAuthenticationStateService, AuthenticationStateService>();
            builder.Services.AddSingleton<IAppDialogService, AppDialogService>();
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            builder.Services.AddHttpClient("PerformIqApi", (serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IAppSettingsService>();
                var baseUrl = settings.GetApiBaseUrl();
                if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
                {
                    uri = new Uri("http://127.0.0.1");
                }

                client.BaseAddress = uri;
            });

            builder.Services.AddSingleton<IHealthConnectBridgeService, HealthConnectBridgeService>();
            builder.Services.AddSingleton<IMobileAuthService, MobileAuthService>();
            builder.Services.AddSingleton<IBackgroundSyncConfigClient, BackgroundSyncConfigClient>();
            builder.Services.AddSingleton<IBackgroundSyncSender, BackgroundSyncSender>();
            builder.Services.AddSingleton<IBackgroundPayloadProducer, HealthConnectBackgroundPayloadProducer>();
            builder.Services.AddSingleton<IBackgroundSyncRunner, BackgroundSyncRunner>();
            builder.Services.AddSingleton<IBackgroundSyncStateStore, BackgroundSyncStateStore>();
#if ANDROID
            builder.Services.AddSingleton<IBackgroundSyncScheduler, AndroidBackgroundSyncScheduler>();
#else
            builder.Services.AddSingleton<IBackgroundSyncScheduler, BackgroundSyncScheduler>();
#endif
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<AuthenticationViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<LogViewModel>();
            builder.Services.AddTransient<AuthenticationPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<LogPage>();

            return builder.Build();
        }
    }
}
