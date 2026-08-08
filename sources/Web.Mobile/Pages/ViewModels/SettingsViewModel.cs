using System.Windows.Input;
using System.Text.Json;
using Web.Mobile.Services;

namespace Web.Mobile.Pages.ViewModels;

public sealed class SettingsViewModel : BaseViewModel
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly IHealthConnectBridgeService _healthConnectBridgeService;
    private readonly IMobileAuthService _mobileAuthService;
    private readonly IAuthenticationStateService _authenticationStateService;
    private readonly ILocalizationService _localizationService;
    private string _apiBaseUrl = string.Empty;
    private string _statusMessage = string.Empty;
    private string _sdkStatusIcon = "❌";
    private string _grantedPermissionsSummary = "0/0";
    private string _selectedLanguageCode = "en";
    private string _savedApiBaseUrl = string.Empty;
    private string _savedLanguageCode = "EN";
    private bool _hasPendingChanges;
    private bool _canLogout;
    private bool _canRequestPermissions;
    private bool _isSavingSettings;

    public SettingsViewModel(
        IAppSettingsService appSettingsService,
        IHealthConnectBridgeService healthConnectBridgeService,
        IMobileAuthService mobileAuthService,
        IAuthenticationStateService authenticationStateService,
        ILocalizationService localizationService)
    {
        _appSettingsService = appSettingsService;
        _healthConnectBridgeService = healthConnectBridgeService;
        _mobileAuthService = mobileAuthService;
        _authenticationStateService = authenticationStateService;
        _localizationService = localizationService;
        _localizationService.LanguageChanged += OnLanguageChanged;
        _authenticationStateService.AuthenticationStateChanged += OnAuthenticationStateChanged;

        _savedApiBaseUrl = _appSettingsService.GetApiBaseUrl();
        _savedLanguageCode = _localizationService.CurrentLanguageCode.ToUpperInvariant();
        ApiBaseUrl = _savedApiBaseUrl;
        SelectedLanguageCode = _savedLanguageCode;
        StatusMessage = T(AppTextKeys.StatusReady);
        SaveSettingsCommand = new Command(SaveSettings, () => HasPendingChanges);
        RequestPermissionsCommand = new Command(async () => await RequestPermissionsAsync(), () => CanRequestPermissions);
        LogoutCommand = new Command(async () => await LogoutAsync(), () => CanLogout);
    }

    public string PageTitle => T(AppTextKeys.LabelSettings);
    public string ApiBaseUrlLabelText => T(AppTextKeys.LabelApiBaseUrl);
    public string ApiBaseUrlPlaceholder => T(AppTextKeys.PlaceholderApiUrl);
    public string SaveSettingsButtonText => T(AppTextKeys.ButtonSaveSettings);
    public string RequestPermissionsButtonText => T(AppTextKeys.ButtonRequestPermissions);
    public string LogoutButtonText => T(AppTextKeys.ButtonLogout);
    public string LanguageLabelText => T(AppTextKeys.LabelLanguage);
    public string SdkStatusLabelText => T(AppTextKeys.LabelSdkStatus);
    public string GrantedPermissionsLabelText => T(AppTextKeys.LabelGrantedPermissions);
    public IReadOnlyList<string> LanguageOptions => ["EN", "DE"];

    public string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set
        {
            if (SetProperty(ref _apiBaseUrl, value))
            {
                if (!_isSavingSettings)
                {
                    UpdateHasPendingChanges();
                }
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string SdkStatusIcon
    {
        get => _sdkStatusIcon;
        set => SetProperty(ref _sdkStatusIcon, value);
    }

    public string GrantedPermissionsSummary
    {
        get => _grantedPermissionsSummary;
        set => SetProperty(ref _grantedPermissionsSummary, value);
    }

    public string SelectedLanguageCode
    {
        get => _selectedLanguageCode;
        set
        {
            if (SetProperty(ref _selectedLanguageCode, value))
            {
                if (!_isSavingSettings)
                {
                    UpdateHasPendingChanges();
                }
            }
        }
    }

    public bool HasPendingChanges
    {
        get => _hasPendingChanges;
        private set
        {
            if (SetProperty(ref _hasPendingChanges, value))
            {
                (SaveSettingsCommand as Command)?.ChangeCanExecute();
            }
        }
    }

    public bool CanLogout
    {
        get => _canLogout;
        private set
        {
            if (SetProperty(ref _canLogout, value))
            {
                (LogoutCommand as Command)?.ChangeCanExecute();
            }
        }
    }

    public bool CanRequestPermissions
    {
        get => _canRequestPermissions;
        private set
        {
            if (SetProperty(ref _canRequestPermissions, value))
            {
                (RequestPermissionsCommand as Command)?.ChangeCanExecute();
            }
        }
    }

    public ICommand SaveSettingsCommand { get; }
    public ICommand RequestPermissionsCommand { get; }
    public ICommand LogoutCommand { get; }

    public async Task InitializeAsync()
    {
        await RefreshSdkStatusAsync();
        await RefreshLogoutAvailabilityAsync();
    }

    private void SaveSettings()
    {
        var currentApi = ApiBaseUrl.Trim();
        var currentLanguage = SelectedLanguageCode.Trim().ToUpperInvariant();
        var apiChanged = !string.Equals(currentApi, _savedApiBaseUrl, StringComparison.Ordinal);
        var languageChanged = !string.Equals(currentLanguage, _savedLanguageCode, StringComparison.Ordinal);

        if (!apiChanged && !languageChanged)
        {
            return;
        }

        if (apiChanged)
        {
            var apiSaved = _appSettingsService.TrySetApiBaseUrl(currentApi);
            if (!apiSaved)
            {
                StatusMessage = T(AppTextKeys.StatusApiUrlInvalid);
                return;
            }

            _savedApiBaseUrl = _appSettingsService.GetApiBaseUrl();
        }

        if (languageChanged)
        {
            var languageCode = currentLanguage.Equals("DE", StringComparison.Ordinal) ? "de" : "en";
            _localizationService.SetLanguage(languageCode);
            _savedLanguageCode = _localizationService.CurrentLanguageCode.ToUpperInvariant();
        }

        _isSavingSettings = true;
        ApiBaseUrl = _savedApiBaseUrl;
        SelectedLanguageCode = _savedLanguageCode;
        _isSavingSettings = false;
        UpdateHasPendingChanges();
        StatusMessage = T(AppTextKeys.StatusSettingsSaved);
    }

    private async Task RefreshSdkStatusAsync()
    {
        var isSupported = _healthConnectBridgeService.IsSdkSupported();
        var isAvailable = await _healthConnectBridgeService.IsHealthConnectAvailableAsync();
        SdkStatusIcon = isSupported && isAvailable ? "✅" : "❌";

        var requiredPermissions = ParsePermissions(await _healthConnectBridgeService.GetRequiredPermissionsJsonAsync());
        var grantedPermissions = ParsePermissions(await _healthConnectBridgeService.GetGrantedPermissionsJsonAsync());
        var requiredCount = requiredPermissions.Count;
        var grantedCount = grantedPermissions.Count(permission => requiredPermissions.Contains(permission));
        var hasAllPermissions = requiredCount > 0 && grantedCount >= requiredCount;

        CanRequestPermissions = isSupported && isAvailable && requiredCount > 0 && !hasAllPermissions;
        GrantedPermissionsSummary = string.Format(T(AppTextKeys.StatusPermissionCountFormat), grantedCount, requiredCount);
    }

    private async Task RequestPermissionsAsync()
    {
        if (!CanRequestPermissions)
        {
            return;
        }

        await _healthConnectBridgeService.RequestAllPermissionsAsync();
        await RefreshSdkStatusAsync();
    }

    private async Task LogoutAsync()
    {
        await _mobileAuthService.LogoutAsync();
        await RefreshLogoutAvailabilityAsync();
    }

    private static HashSet<string> ParsePermissions(string permissionsJson)
    {
        var permissions = JsonSerializer.Deserialize<string[]>(permissionsJson);
        return permissions is null
            ? []
            : permissions
                .Where(permission => !string.IsNullOrWhiteSpace(permission))
                .ToHashSet(StringComparer.Ordinal);
    }

    private void UpdateHasPendingChanges()
    {
        var currentApi = ApiBaseUrl.Trim();
        var currentLanguage = SelectedLanguageCode.Trim().ToUpperInvariant();
        HasPendingChanges = !string.Equals(currentApi, _savedApiBaseUrl, StringComparison.Ordinal)
                            || !string.Equals(currentLanguage, _savedLanguageCode, StringComparison.Ordinal);
    }

    private async Task RefreshLogoutAvailabilityAsync()
    {
        var hasRefreshToken = await _mobileAuthService.HasRefreshTokenAsync();
        CanLogout = _authenticationStateService.IsAuthenticated && hasRefreshToken;
    }

    private string T(string key) => _localizationService.Get(key);

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(ApiBaseUrlLabelText));
        OnPropertyChanged(nameof(ApiBaseUrlPlaceholder));
        OnPropertyChanged(nameof(SaveSettingsButtonText));
        OnPropertyChanged(nameof(RequestPermissionsButtonText));
        OnPropertyChanged(nameof(LogoutButtonText));
        OnPropertyChanged(nameof(LanguageLabelText));
        OnPropertyChanged(nameof(SdkStatusLabelText));
        OnPropertyChanged(nameof(GrantedPermissionsLabelText));
        var selected = _localizationService.CurrentLanguageCode.ToUpperInvariant();
        if (_isSavingSettings)
        {
            return;
        }

        if (!HasPendingChanges)
        {
            _savedLanguageCode = selected;
        }
        SelectedLanguageCode = selected;
    }

    private void OnAuthenticationStateChanged(object? sender, bool isAuthenticated)
    {
        MainThread.BeginInvokeOnMainThread(async () => await RefreshLogoutAvailabilityAsync());
    }
}
