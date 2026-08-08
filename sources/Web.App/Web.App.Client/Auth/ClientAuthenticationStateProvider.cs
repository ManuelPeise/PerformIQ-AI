using Microsoft.AspNetCore.Components.Authorization;

namespace Web.App.Client.Auth;

public sealed class ClientAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ClientAuthSessionService _authSessionService;

    public ClientAuthenticationStateProvider(ClientAuthSessionService authSessionService)
    {
        _authSessionService = authSessionService;
        _authSessionService.SessionChanged += OnSessionChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_authSessionService.CurrentPrincipal));
    }

    private void OnSessionChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
