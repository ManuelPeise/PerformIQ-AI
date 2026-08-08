using System.Net.Http.Json;
using System.Text.Json;
using Shared.Models.Authentication;

namespace Web.Mobile.Services;

public sealed class MobileAuthService : IMobileAuthService
{
    private const string RefreshTokenKey = "mobile.refresh-token";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthenticationStateService _authenticationStateService;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private AuthResponseModel? _currentSession;

    public MobileAuthService(
        IHttpClientFactory httpClientFactory,
        IAuthenticationStateService authenticationStateService)
    {
        _httpClientFactory = httpClientFactory;
        _authenticationStateService = authenticationStateService;
    }

    public async Task<AuthResponseModel> LoginAsync(LoginRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient("PerformIqApi");
        using var response = await client.PostAsJsonAsync("/api/mobile/auth/login", requestModel, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var messageKey = await ReadErrorMessageKeyAsync(response, AppTextKeys.AuthMobileLoginFailed);
            throw new MobileAuthException(messageKey);
        }

        var session = await response.Content.ReadFromJsonAsync<AuthResponseModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty login response.");
        await SaveSessionAsync(session);
        return session;
    }

    public async Task<string?> GetValidAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (HasValidAccessToken())
        {
            return _currentSession!.AccessToken;
        }

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            if (HasValidAccessToken())
            {
                return _currentSession!.AccessToken;
            }

            var refreshed = await TryRefreshAsync(cancellationToken);
            return refreshed ? _currentSession?.AccessToken : null;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<bool> RestoreSessionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetValidAccessTokenAsync(cancellationToken);
            return !string.IsNullOrWhiteSpace(accessToken);
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    public async Task<bool> HasRefreshTokenAsync()
    {
        var token = await SecureStorage.Default.GetAsync(RefreshTokenKey);
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task LogoutAsync()
    {
        _currentSession = null;
        SecureStorage.Default.Remove(RefreshTokenKey);
        _authenticationStateService.SetAuthenticated(false);
        await Task.CompletedTask;
    }

    private bool HasValidAccessToken()
    {
        if (_currentSession is null)
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(_currentSession.AccessToken)
               && _currentSession.AccessTokenExpiresAtUtc > DateTime.UtcNow.AddMinutes(1);
    }

    private async Task<bool> TryRefreshAsync(CancellationToken cancellationToken)
    {
        var refreshToken = await SecureStorage.Default.GetAsync(RefreshTokenKey);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        using var client = _httpClientFactory.CreateClient("PerformIqApi");
        using var response = await client.PostAsJsonAsync(
            "/api/mobile/auth/refresh",
            new RefreshTokenRequestModel { RefreshToken = refreshToken },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await LogoutAsync();
            return false;
        }

        var session = await response.Content.ReadFromJsonAsync<AuthResponseModel>(cancellationToken: cancellationToken);
        if (session is null || string.IsNullOrWhiteSpace(session.RefreshToken))
        {
            await LogoutAsync();
            return false;
        }

        await SaveSessionAsync(session);
        return true;
    }

    private async Task SaveSessionAsync(AuthResponseModel session)
    {
        _currentSession = session;
        await SecureStorage.Default.SetAsync(RefreshTokenKey, session.RefreshToken);
        _authenticationStateService.SetAuthenticated(true);
    }

    private static async Task<string> ReadErrorMessageKeyAsync(HttpResponseMessage response, string fallback)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return fallback;
            }

            var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
            if (payload is not null
                && payload.TryGetValue("messageKey", out var messageKey)
                && !string.IsNullOrWhiteSpace(messageKey))
            {
                return messageKey;
            }
        }
        catch
        {
            return fallback;
        }

        return fallback;
    }
}
