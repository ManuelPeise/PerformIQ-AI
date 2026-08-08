using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Web.Mobile.Services;

namespace Web.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private AppShell? _appShell;
    private int _sessionRestoreStarted;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        _appShell ??= _serviceProvider.GetRequiredService<AppShell>();
        var window = new Window(_appShell);

        if (Interlocked.Exchange(ref _sessionRestoreStarted, 1) == 0)
        {
            _ = RestoreSessionAsync();
        }

        return window;
    }

    private async Task RestoreSessionAsync()
    {
        var authService = _serviceProvider.GetRequiredService<IMobileAuthService>();
        await authService.RestoreSessionAsync();
    }
}