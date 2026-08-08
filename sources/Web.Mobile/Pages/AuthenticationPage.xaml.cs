using Web.Mobile.Pages.ViewModels;

namespace Web.Mobile.Pages;

public partial class AuthenticationPage : ContentPage
{
    public AuthenticationPage(AuthenticationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
