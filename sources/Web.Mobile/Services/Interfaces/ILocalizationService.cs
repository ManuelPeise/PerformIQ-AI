namespace Web.Mobile.Services;

public interface ILocalizationService
{
    string CurrentLanguageCode { get; }
    IReadOnlyList<string> SupportedLanguageCodes { get; }
    string Get(string key);
    void SetLanguage(string languageCode);
    event EventHandler? LanguageChanged;
}
