using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Shared.Models.Authentication;
using Web.App.Client.Auth;

namespace Web.App.Client.Pages.Auth.ViewModels;

public sealed class RegisterViewModel
{
    private readonly IClientAuthenticationService _authenticationService;
    private readonly NavigationManager _navigationManager;

    public RegisterFormModel Model { get; } = new();
    public bool IsSubmitting { get; private set; }
    public string? ErrorMessage { get; private set; }

    public RegisterViewModel(IClientAuthenticationService authenticationService, NavigationManager navigationManager)
    {
        _authenticationService = authenticationService;
        _navigationManager = navigationManager;
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
            await ExecuteRegistrationAsync();
            _navigationManager.NavigateTo("/home", forceLoad: false);
        }
        catch (Exception exception) when (IsExpectedRegistrationException(exception))
        {
            ErrorMessage = "Registration failed. Verify your input and try again.";
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

    private async Task ExecuteRegistrationAsync()
    {
        var request = new RegisterRequestModel
        {
            UserName = Model.UserName,
            Email = Model.Email,
            Password = Model.Password
        };
        await _authenticationService.RegisterAsync(request);
    }

    private static bool IsExpectedRegistrationException(Exception exception)
    {
        return exception is HttpRequestException or InvalidOperationException;
    }
}

public sealed class RegisterFormModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
