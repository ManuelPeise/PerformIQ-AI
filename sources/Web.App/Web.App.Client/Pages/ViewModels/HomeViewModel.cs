using Shared.Models.Profile;
using Web.App.Client.Auth;

namespace Web.App.Client.Pages.ViewModels;

public sealed class HomeViewModel
{
    private readonly AuthHttpClient _authHttpClient;

    public UserProfileModel? Profile { get; private set; }
    public bool IsLoading { get; private set; } = true;
    public string? ErrorMessage { get; private set; }

    public HomeViewModel(AuthHttpClient authHttpClient)
    {
        _authHttpClient = authHttpClient;
    }

    public async Task LoadAsync()
    {
        BeginLoading();

        try
        {
            Profile = await _authHttpClient.GetFromJsonAsync<UserProfileModel>("/api/profile/");
        }
        catch (Exception exception) when (IsExpectedProfileException(exception))
        {
            ErrorMessage = "Unable to load your profile.";
        }
        finally
        {
            EndLoading();
        }
    }

    private void BeginLoading()
    {
        IsLoading = true;
        ErrorMessage = null;
    }

    private void EndLoading()
    {
        IsLoading = false;
    }

    private static bool IsExpectedProfileException(Exception exception)
    {
        return exception is HttpRequestException or InvalidOperationException;
    }
}
