using Web.Mobile.Services;

namespace Web.Mobile.Pages;

public partial class SplashPage : ContentPage
{
    private readonly ILocalizationService _localizationService;
    public event EventHandler? Completed;

    public SplashPage(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        InitializeComponent();
        AppNameLabel.Text = _localizationService.Get(AppTextKeys.AppName);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(1200);
        Completed?.Invoke(this, EventArgs.Empty);
    }
}
