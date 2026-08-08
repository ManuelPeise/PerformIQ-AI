using System.Net.Http.Json;
using Shared.Models.Authentication;

namespace Web.App.Client.Auth;

public sealed class ClientAuthenticationService : IClientAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ClientAuthSessionService _authSessionService;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public ClientAuthenticationService(HttpClient httpClient, ClientAuthSessionService authSessionService)
    {
        _httpClient = httpClient;
        _authSessionService = authSessionService;
    }

    public async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
    {
        return await TryRefreshAsync(cancellationToken);
    }

    public async Task<AuthSessionResponseModel> LoginAsync(LoginRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        return await ExecuteAuthRequestAsync("/api/auth/login", requestModel, "Empty login response.", cancellationToken);
    }

    public async Task<AuthSessionResponseModel> RegisterAsync(RegisterRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        return await ExecuteAuthRequestAsync("/api/auth/register", requestModel, "Empty register response.", cancellationToken);
    }

    public async Task<bool> TryRefreshAsync(CancellationToken cancellationToken = default)
    {
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            return await ExecuteRefreshAsync(cancellationToken);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        _authSessionService.ClearSession();
        using var _ = await _httpClient.PostAsJsonAsync("/api/auth/revoke", new RevokeTokenRequestModel(), cancellationToken);
    }

    private async Task<AuthSessionResponseModel> ExecuteAuthRequestAsync<TRequest>(
        string uri,
        TRequest requestModel,
        string emptyResponseError,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(uri, requestModel, cancellationToken);
        response.EnsureSuccessStatusCode();

        var session = await response.Content.ReadFromJsonAsync<AuthSessionResponseModel>(cancellationToken: cancellationToken);
        if (session is null)
        {
            throw new InvalidOperationException(emptyResponseError);
        }

        _authSessionService.SetSession(session);
        return session;
    }

    private async Task<bool> ExecuteRefreshAsync(CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequestModel(), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _authSessionService.ClearSession();
            return false;
        }

        var session = await response.Content.ReadFromJsonAsync<AuthSessionResponseModel>(cancellationToken: cancellationToken);
        if (session is null)
        {
            _authSessionService.ClearSession();
            return false;
        }

        _authSessionService.SetSession(session);
        return true;
    }
}
