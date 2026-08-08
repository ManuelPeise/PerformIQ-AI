namespace Web.Mobile.Services;

public interface IAppSettingsService
{
    event EventHandler? ApiBaseUrlChanged;
    string GetApiBaseUrl();
    bool TrySetApiBaseUrl(string baseUrl);
    bool HasApiBaseUrl();
    string GetLanguageCode();
    void SetLanguageCode(string languageCode);
}
