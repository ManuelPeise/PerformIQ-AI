using Web.Mobile.Pages;
using Web.Mobile.Services;

namespace Web.Mobile;

public partial class AppShell : Shell
{
    private readonly IAuthenticationStateService _authenticationStateService;
    private readonly ILocalizationService _localizationService;
    private readonly AuthenticationPage _authenticationPage;
    private readonly SettingsPage _settingsPage;
    private readonly LogPage _logPage;

    public AppShell(
        IAuthenticationStateService authenticationStateService,
        ILocalizationService localizationService,
        AuthenticationPage authenticationPage,
        SettingsPage settingsPage,
        LogPage logPage)
    {
        _authenticationStateService = authenticationStateService;
        _localizationService = localizationService;
        _authenticationPage = authenticationPage;
        _settingsPage = settingsPage;
        _logPage = logPage;

        InitializeComponent();
        BuildTabs();
        _authenticationStateService.AuthenticationStateChanged += OnAuthenticationStateChanged;
        _localizationService.LanguageChanged += OnLanguageChanged;
    }

    private void BuildTabs()
    {
        var tabBar = new TabBar();
        if (!_authenticationStateService.IsAuthenticated)
        {
            tabBar.Items.Add(new ShellContent
            {
                Title = _localizationService.Get(AppTextKeys.TabAuthentication),
                Content = _authenticationPage,
                Route = nameof(AuthenticationPage)
            });
        }

        tabBar.Items.Add(new ShellContent
        {
            Title = _localizationService.Get(AppTextKeys.TabSettings),
            Content = _settingsPage,
            Route = nameof(SettingsPage)
        });

        if (_authenticationStateService.IsAuthenticated)
        {
            tabBar.Items.Add(new ShellContent
            {
                Title = _localizationService.Get(AppTextKeys.TabLog),
                Content = _logPage,
                Route = nameof(LogPage)
            });
        }

        Title = _localizationService.Get(AppTextKeys.AppName);
        Items.Clear();
        Items.Add(tabBar);
    }

    private void OnAuthenticationStateChanged(object? sender, bool isAuthenticated)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            BuildTabs();
            if (!isAuthenticated)
            {
                await GoToAsync($"//{nameof(AuthenticationPage)}");
            }
        });
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(BuildTabs);
    }
}
