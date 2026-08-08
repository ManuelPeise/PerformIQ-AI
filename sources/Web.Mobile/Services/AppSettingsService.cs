namespace Web.Mobile.Services;

public sealed class AppSettingsService : IAppSettingsService
{
    private const string ApiBaseUrlKey = "settings.api-base-url";
    private const string LanguageCodeKey = "settings.language-code";
    public event EventHandler? ApiBaseUrlChanged;

    public string GetApiBaseUrl()
    {
        return Preferences.Default.Get(ApiBaseUrlKey, string.Empty).Trim();
    }

    public bool TrySetApiBaseUrl(string baseUrl)
    {
        var normalized = baseUrl.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            Preferences.Default.Set(ApiBaseUrlKey, string.Empty);
            ApiBaseUrlChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        if (!Uri.TryCreate(normalized, UriKind.Absolute, out _))
        {
            return false;
        }

        Preferences.Default.Set(ApiBaseUrlKey, normalized.TrimEnd('/'));
        ApiBaseUrlChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool HasApiBaseUrl()
    {
        var value = Preferences.Default.Get(ApiBaseUrlKey, string.Empty).Trim();
        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }

    public string GetLanguageCode()
    {
        var value = Preferences.Default.Get(LanguageCodeKey, "en").Trim().ToLowerInvariant();
        return value is "de" ? "de" : "en";
    }

    public void SetLanguageCode(string languageCode)
    {
        var value = languageCode.Trim().ToLowerInvariant();
        Preferences.Default.Set(LanguageCodeKey, value is "de" ? "de" : "en");
    }
}
