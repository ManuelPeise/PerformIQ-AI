using System.Windows.Input;
using Shared.Models.Authentication;
using Web.Mobile.Services;

namespace Web.Mobile.Pages.ViewModels;

public sealed class AuthenticationViewModel : BaseViewModel
{
    private readonly IMobileAuthService _mobileAuthService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IBackgroundSyncRunner _backgroundSyncRunner;
    private readonly IAppDialogService _appDialogService;
    private readonly ILocalizationService _localizationService;
    private string _userNameOrEmail = string.Empty;
    private string _password = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isBusy;

    public AuthenticationViewModel(
        IMobileAuthService mobileAuthService,
        IAppSettingsService appSettingsService,
        IBackgroundSyncRunner backgroundSyncRunner,
        IAppDialogService appDialogService,
        ILocalizationService localizationService)
    {
        _mobileAuthService = mobileAuthService;
        _appSettingsService = appSettingsService;
        _backgroundSyncRunner = backgroundSyncRunner;
        _appDialogService = appDialogService;
        _localizationService = localizationService;
        _appSettingsService.ApiBaseUrlChanged += OnApiBaseUrlChanged;
        _localizationService.LanguageChanged += OnLanguageChanged;
        StatusMessage = string.Empty;
        LoginCommand = new Command(async () => await LoginAsync(), () => CanLogin);
    }

    public string PageTitle => T(AppTextKeys.LabelAuth);
    public string UserNameLabel => T(AppTextKeys.LabelUsername);
    public string PasswordLabel => T(AppTextKeys.LabelPassword);
    public string UserNamePlaceholder => T(AppTextKeys.PlaceholderUsername);
    public string PasswordPlaceholder => T(AppTextKeys.PlaceholderPassword);
    public string LoginButtonText => T(AppTextKeys.ButtonLogin);
    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);
    public bool CanLogin =>
        !IsBusy &&
        _appSettingsService.HasApiBaseUrl() &&
        !string.IsNullOrWhiteSpace(UserNameOrEmail) &&
        !string.IsNullOrWhiteSpace(Password);

    public string UserNameOrEmail
    {
        get => _userNameOrEmail;
        set
        {
            if (SetProperty(ref _userNameOrEmail, value))
            {
                OnPropertyChanged(nameof(CanLogin));
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                OnPropertyChanged(nameof(CanLogin));
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (SetProperty(ref _statusMessage, value))
            {
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(CanLogin));
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        if (!_appSettingsService.HasApiBaseUrl())
        {
            StatusMessage = T(AppTextKeys.ErrorConfigureApiUrl);
            await _appDialogService.ShowMessageAsync(T(AppTextKeys.DialogLoginBlockedTitle), StatusMessage);
            return;
        }

        if (string.IsNullOrWhiteSpace(UserNameOrEmail) || string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = T(AppTextKeys.ErrorEnterCredentials);
            await _appDialogService.ShowMessageAsync(T(AppTextKeys.DialogLoginBlockedTitle), StatusMessage);
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            await _mobileAuthService.LoginAsync(new LoginRequestModel
            {
                UserNameOrEmail = UserNameOrEmail.Trim(),
                Password = Password
            });

            await _backgroundSyncRunner.ExecuteAsync();
            StatusMessage = string.Empty;
        }
        catch (MobileAuthException ex)
        {
            var message = T(ex.MessageKey);
            StatusMessage = message;
            await _appDialogService.ShowMessageAsync(T(AppTextKeys.DialogAuthFailedTitle), message);
        }
        catch (Exception)
        {
            var message = T(AppTextKeys.ErrorUnexpectedLogin);
            StatusMessage = message;
            await _appDialogService.ShowMessageAsync(T(AppTextKeys.DialogAuthFailedTitle), message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string T(string key) => _localizationService.Get(key);

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(UserNameLabel));
        OnPropertyChanged(nameof(PasswordLabel));
        OnPropertyChanged(nameof(UserNamePlaceholder));
        OnPropertyChanged(nameof(PasswordPlaceholder));
        OnPropertyChanged(nameof(LoginButtonText));
    }

    private void OnApiBaseUrlChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(CanLogin));
        (LoginCommand as Command)?.ChangeCanExecute();
    }
}
