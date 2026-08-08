using Web.Mobile.Pages.ViewModels;

namespace Web.Mobile.Pages;

public partial class LogPage : ContentPage
{
    private readonly LogViewModel _viewModel;

    public LogPage(LogViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.RefreshAsync();
    }
}
