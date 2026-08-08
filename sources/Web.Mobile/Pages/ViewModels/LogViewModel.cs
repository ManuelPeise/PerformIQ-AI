using System.Windows.Input;
using Web.Mobile.Services;

namespace Web.Mobile.Pages.ViewModels;

public sealed class LogViewModel : BaseViewModel
{
    private readonly IHealthConnectBridgeService _healthConnectBridgeService;
    private readonly IBackgroundSyncStateStore _backgroundSyncStateStore;
    private readonly ILocalizationService _localizationService;
    private string _sdkAvailabilityIcon = "❔";
    private string _sdkAvailabilityText = string.Empty;
    private string _grantedPermissions = "[]";
    private string _backgroundInfo = string.Empty;

    public LogViewModel(
        IHealthConnectBridgeService healthConnectBridgeService,
        IBackgroundSyncStateStore backgroundSyncStateStore,
        ILocalizationService localizationService)
    {
        _healthConnectBridgeService = healthConnectBridgeService;
        _backgroundSyncStateStore = backgroundSyncStateStore;
        _localizationService = localizationService;
        _localizationService.LanguageChanged += OnLanguageChanged;
        _sdkAvailabilityText = T(AppTextKeys.StatusUnavailable);
        _backgroundInfo = T(AppTextKeys.StatusUnavailable);
        RefreshCommand = new Command(async () => await RefreshAsync());
    }

    public string PageTitle => T(AppTextKeys.LabelLog);
    public string HealthConnectSdkLabel => T(AppTextKeys.LabelHealthConnectSdk);
    public string GrantedPermissionsLabel => T(AppTextKeys.LabelGrantedPermissions);
    public string BackgroundTaskInfoLabel => T(AppTextKeys.LabelBackgroundTaskInfo);
    public string RefreshButtonText => T(AppTextKeys.ButtonRefresh);

    public string SdkAvailabilityIcon
    {
        get => _sdkAvailabilityIcon;
        set => SetProperty(ref _sdkAvailabilityIcon, value);
    }

    public string SdkAvailabilityText
    {
        get => _sdkAvailabilityText;
        set => SetProperty(ref _sdkAvailabilityText, value);
    }

    public string GrantedPermissions
    {
        get => _grantedPermissions;
        set => SetProperty(ref _grantedPermissions, value);
    }

    public string BackgroundInfo
    {
        get => _backgroundInfo;
        set => SetProperty(ref _backgroundInfo, value);
    }

    public ICommand RefreshCommand { get; }

    public async Task RefreshAsync()
    {
        var isAvailable = await _healthConnectBridgeService.IsHealthConnectAvailableAsync();
        SdkAvailabilityIcon = isAvailable ? "✅" : "❌";
        SdkAvailabilityText = isAvailable ? T(AppTextKeys.StatusAvailable) : T(AppTextKeys.StatusUnavailable);
        GrantedPermissions = await _healthConnectBridgeService.GetGrantedPermissionsJsonAsync();

        var snapshot = _backgroundSyncStateStore.GetSnapshot();
        BackgroundInfo = string.Format(
            T(AppTextKeys.StatusBackgroundInfoFormat),
            snapshot.IsActive,
            snapshot.IntervalMinutes,
            snapshot.LastStatus,
            snapshot.LastRunAtUtc?.ToString("O") ?? "-");
    }

    private string T(string key) => _localizationService.Get(key);

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(HealthConnectSdkLabel));
        OnPropertyChanged(nameof(GrantedPermissionsLabel));
        OnPropertyChanged(nameof(BackgroundTaskInfoLabel));
        OnPropertyChanged(nameof(RefreshButtonText));
    }
}
