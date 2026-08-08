namespace Web.Mobile.Services;

public sealed class LocalizationService : ILocalizationService
{
    private static readonly IReadOnlyDictionary<string, string> En = new Dictionary<string, string>
    {
        [AppTextKeys.AppName] = "PerformIq Ai",
        [AppTextKeys.TabAuthentication] = "Authentication",
        [AppTextKeys.TabSettings] = "Settings",
        [AppTextKeys.TabLog] = "Log",
        [AppTextKeys.ButtonLogin] = "Login",
        [AppTextKeys.ButtonSaveApiUrl] = "Save API URL",
        [AppTextKeys.ButtonSaveSettings] = "Save Settings",
        [AppTextKeys.ButtonCheckSdk] = "Check HealthConnect SDK",
        [AppTextKeys.ButtonRequestPermissions] = "Request Health Connect Permissions",
        [AppTextKeys.ButtonLogout] = "Logout",
        [AppTextKeys.ButtonRefresh] = "Refresh",
        [AppTextKeys.ButtonSaveLanguage] = "Save Language",
        [AppTextKeys.PlaceholderUsername] = "Username or email",
        [AppTextKeys.PlaceholderPassword] = "Password",
        [AppTextKeys.PlaceholderApiUrl] = "API base URL",
        [AppTextKeys.LabelSettings] = "Settings",
        [AppTextKeys.LabelLog] = "Log",
        [AppTextKeys.LabelAuth] = "Authentication",
        [AppTextKeys.LabelUsername] = "Username or email",
        [AppTextKeys.LabelPassword] = "Password",
        [AppTextKeys.LabelLanguage] = "Language",
        [AppTextKeys.LabelApiBaseUrl] = "Api Base URL",
        [AppTextKeys.LabelSdkStatus] = "Sdk Status:",
        [AppTextKeys.LabelHealthConnectSdk] = "HealthConnect SDK:",
        [AppTextKeys.LabelGrantedPermissions] = "Granted Permissions:",
        [AppTextKeys.LabelBackgroundTaskInfo] = "Background Task Info:",
        [AppTextKeys.StatusNotAuthenticated] = "Not authenticated.",
        [AppTextKeys.StatusLoggingIn] = "Logging in...",
        [AppTextKeys.StatusLoginSuccessSyncOk] = "Login successful. Background sync bootstrap succeeded.",
        [AppTextKeys.StatusLoginSuccessSyncFailed] = "Login successful. Background sync bootstrap failed.",
        [AppTextKeys.StatusApiUrlSaved] = "API base URL saved.",
        [AppTextKeys.StatusSettingsSaved] = "Settings saved.",
        [AppTextKeys.StatusApiUrlInvalid] = "Invalid API base URL.",
        [AppTextKeys.StatusSdkUpdated] = "SDK status updated.",
        [AppTextKeys.StatusPermissionCompleted] = "Permission request completed.",
        [AppTextKeys.StatusPermissionGranted] = "Granted",
        [AppTextKeys.StatusPermissionMissing] = "Missing or canceled",
        [AppTextKeys.StatusReady] = "Ready.",
        [AppTextKeys.StatusAvailable] = "Available",
        [AppTextKeys.StatusUnavailable] = "Unavailable",
        [AppTextKeys.StatusPermissionCountFormat] = "{0}/{1}",
        [AppTextKeys.StatusSdkSummary] = "Supported={0}, Available={1}",
        [AppTextKeys.StatusBackgroundInfoFormat] = "Active={0}\nIntervalMinutes={1}\nLastStatus={2}\nLastRunAtUtc={3}",
        [AppTextKeys.DialogLoginBlockedTitle] = "Login blocked",
        [AppTextKeys.DialogAuthFailedTitle] = "Authentication failed",
        [AppTextKeys.ErrorConfigureApiUrl] = "Configure API base URL in Settings before login.",
        [AppTextKeys.ErrorEnterCredentials] = "Enter username/email and password.",
        [AppTextKeys.ErrorUnexpectedLogin] = "Unexpected error during login.",
        [AppTextKeys.AuthMobileRoleNotAllowedAdmin] = "Admin users are not allowed to authenticate from the mobile app.",
        [AppTextKeys.AuthMobileRoleNotAllowedSystemAdmin] = "System admin users are not allowed to authenticate from the mobile app.",
        [AppTextKeys.AuthMobileRoleNotAllowedGuest] = "Guest users are not allowed to authenticate from the mobile app.",
        [AppTextKeys.AuthMobileInvalidCredentials] = "Invalid credentials.",
        [AppTextKeys.AuthMobileInvalidRequest] = "Invalid authentication request.",
        [AppTextKeys.AuthMobileLoginFailed] = "Login failed."
    };

    private static readonly IReadOnlyDictionary<string, string> De = new Dictionary<string, string>
    {
        [AppTextKeys.AppName] = "PerformIq Ai",
        [AppTextKeys.TabAuthentication] = "Anmeldung",
        [AppTextKeys.TabSettings] = "Einstellungen",
        [AppTextKeys.TabLog] = "Protokoll",
        [AppTextKeys.ButtonLogin] = "Anmelden",
        [AppTextKeys.ButtonSaveApiUrl] = "API-URL speichern",
        [AppTextKeys.ButtonSaveSettings] = "Einstellungen speichern",
        [AppTextKeys.ButtonCheckSdk] = "HealthConnect SDK prüfen",
        [AppTextKeys.ButtonRequestPermissions] = "Health Connect-Berechtigungen anfordern",
        [AppTextKeys.ButtonLogout] = "Abmelden",
        [AppTextKeys.ButtonRefresh] = "Aktualisieren",
        [AppTextKeys.ButtonSaveLanguage] = "Sprache speichern",
        [AppTextKeys.PlaceholderUsername] = "Benutzername oder E-Mail",
        [AppTextKeys.PlaceholderPassword] = "Passwort",
        [AppTextKeys.PlaceholderApiUrl] = "API-Basis-URL",
        [AppTextKeys.LabelSettings] = "Einstellungen",
        [AppTextKeys.LabelLog] = "Protokoll",
        [AppTextKeys.LabelAuth] = "Anmeldung",
        [AppTextKeys.LabelUsername] = "Benutzername oder E-Mail",
        [AppTextKeys.LabelPassword] = "Passwort",
        [AppTextKeys.LabelLanguage] = "Sprache",
        [AppTextKeys.LabelApiBaseUrl] = "API-Basis-URL",
        [AppTextKeys.LabelSdkStatus] = "SDK-Status:",
        [AppTextKeys.LabelHealthConnectSdk] = "HealthConnect SDK:",
        [AppTextKeys.LabelGrantedPermissions] = "Erteilte Berechtigungen:",
        [AppTextKeys.LabelBackgroundTaskInfo] = "Hintergrundaufgaben-Info:",
        [AppTextKeys.StatusNotAuthenticated] = "Nicht angemeldet.",
        [AppTextKeys.StatusLoggingIn] = "Anmeldung läuft...",
        [AppTextKeys.StatusLoginSuccessSyncOk] = "Anmeldung erfolgreich. Hintergrundabgleich gestartet.",
        [AppTextKeys.StatusLoginSuccessSyncFailed] = "Anmeldung erfolgreich, aber Hintergrundabgleich fehlgeschlagen.",
        [AppTextKeys.StatusApiUrlSaved] = "API-Basis-URL gespeichert.",
        [AppTextKeys.StatusSettingsSaved] = "Einstellungen gespeichert.",
        [AppTextKeys.StatusApiUrlInvalid] = "Ungültige API-Basis-URL.",
        [AppTextKeys.StatusSdkUpdated] = "SDK-Status aktualisiert.",
        [AppTextKeys.StatusPermissionCompleted] = "Berechtigungsanfrage abgeschlossen.",
        [AppTextKeys.StatusPermissionGranted] = "Erteilt",
        [AppTextKeys.StatusPermissionMissing] = "Fehlt oder abgebrochen",
        [AppTextKeys.StatusReady] = "Bereit.",
        [AppTextKeys.StatusAvailable] = "Verfügbar",
        [AppTextKeys.StatusUnavailable] = "Nicht verfügbar",
        [AppTextKeys.StatusPermissionCountFormat] = "{0}/{1}",
        [AppTextKeys.StatusSdkSummary] = "Unterstützt={0}, Verfügbar={1}",
        [AppTextKeys.StatusBackgroundInfoFormat] = "Aktiv={0}\nIntervallMinuten={1}\nLetzterStatus={2}\nLetzterLaufUtc={3}",
        [AppTextKeys.DialogLoginBlockedTitle] = "Anmeldung blockiert",
        [AppTextKeys.DialogAuthFailedTitle] = "Anmeldung fehlgeschlagen",
        [AppTextKeys.ErrorConfigureApiUrl] = "Bitte zuerst die API-Basis-URL in den Einstellungen konfigurieren.",
        [AppTextKeys.ErrorEnterCredentials] = "Bitte Benutzername/E-Mail und Passwort eingeben.",
        [AppTextKeys.ErrorUnexpectedLogin] = "Unerwarteter Fehler bei der Anmeldung.",
        [AppTextKeys.AuthMobileRoleNotAllowedAdmin] = "Admin-Benutzer dürfen sich in der mobilen App nicht anmelden.",
        [AppTextKeys.AuthMobileRoleNotAllowedSystemAdmin] = "System-Admin-Benutzer dürfen sich in der mobilen App nicht anmelden.",
        [AppTextKeys.AuthMobileRoleNotAllowedGuest] = "Gast-Benutzer dürfen sich in der mobilen App nicht anmelden.",
        [AppTextKeys.AuthMobileInvalidCredentials] = "Ungültige Anmeldedaten.",
        [AppTextKeys.AuthMobileInvalidRequest] = "Ungültige Anmeldeanfrage.",
        [AppTextKeys.AuthMobileLoginFailed] = "Anmeldung fehlgeschlagen."
    };

    private readonly IAppSettingsService _appSettingsService;
    private string _currentLanguageCode;

    public LocalizationService(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
        _currentLanguageCode = _appSettingsService.GetLanguageCode();
    }

    public string CurrentLanguageCode => _currentLanguageCode;

    public IReadOnlyList<string> SupportedLanguageCodes => ["en", "de"];

    public event EventHandler? LanguageChanged;

    public string Get(string key)
    {
        var table = _currentLanguageCode == "de" ? De : En;
        if (table.TryGetValue(key, out var value))
        {
            return value;
        }

        return En.TryGetValue(key, out var fallback) ? fallback : key;
    }

    public void SetLanguage(string languageCode)
    {
        var normalized = languageCode.Trim().ToLowerInvariant();
        var newCode = normalized == "de" ? "de" : "en";
        if (newCode == _currentLanguageCode)
        {
            return;
        }

        _currentLanguageCode = newCode;
        _appSettingsService.SetLanguageCode(newCode);
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
