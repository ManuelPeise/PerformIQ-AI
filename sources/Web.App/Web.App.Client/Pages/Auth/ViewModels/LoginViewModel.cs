using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Shared.Models.Authentication;
using Web.App.Client.Auth;

namespace Web.App.Client.Pages.Auth.ViewModels;

public sealed class LoginViewModel
{
    private readonly IClientAuthenticationService _authenticationService;
    private readonly NavigationManager _navigationManager;
    private string _returnUrl = "/home";

    public LoginFormModel Model { get; } = new();
    public bool IsSubmitting { get; private set; }
    public string? ErrorMessage { get; private set; }

    public LoginViewModel(IClientAuthenticationService authenticationService, NavigationManager navigationManager)
    {
        _authenticationService = authenticationService;
        _navigationManager = navigationManager;
    }

    public void SetReturnUrl(string? returnUrl)
    {
        _returnUrl = BuildSafeReturnUrl(returnUrl);
    }

    public async Task SubmitAsync()
    {
        if (IsSubmitting)
        {
            return;
        }

        await SubmitInternalAsync();
    }

    private async Task SubmitInternalAsync()
    {
        BeginSubmission();

        try
        {
            await ExecuteLoginAsync();
            _navigationManager.NavigateTo(_returnUrl, forceLoad: false);
        }
        catch (Exception exception) when (IsExpectedLoginException(exception))
        {
            ErrorMessage = "Login failed. Check your credentials and try again.";
        }
        finally
        {
            EndSubmission();
        }
    }

    private void BeginSubmission()
    {
        IsSubmitting = true;
        ErrorMessage = null;
    }

    private void EndSubmission()
    {
        IsSubmitting = false;
    }

    private async Task ExecuteLoginAsync()
    {
        var request = new LoginRequestModel { UserNameOrEmail = Model.UserNameOrEmail, Password = Model.Password };
        await _authenticationService.LoginAsync(request);
    }

    private static bool IsExpectedLoginException(Exception exception)
    {
        return exception is HttpRequestException or InvalidOperationException;
    }

    private static string BuildSafeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return "/home";
        }

        var isInvalidReturnUrl = !returnUrl.StartsWith("/", StringComparison.Ordinal)
                                 || returnUrl.StartsWith("//", StringComparison.Ordinal);
        return isInvalidReturnUrl ? "/home" : returnUrl;
    }
}

public sealed class LoginFormModel
{
    [Required]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
