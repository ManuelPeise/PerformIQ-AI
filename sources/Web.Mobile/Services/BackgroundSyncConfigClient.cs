using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shared.Models.Mobile;

namespace Web.Mobile.Services;

public sealed class BackgroundSyncConfigClient : IBackgroundSyncConfigClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMobileAuthService _mobileAuthService;

    public BackgroundSyncConfigClient(IHttpClientFactory httpClientFactory, IMobileAuthService mobileAuthService)
    {
        _httpClientFactory = httpClientFactory;
        _mobileAuthService = mobileAuthService;
    }

    public async Task<BackgroundSyncConfigResponseModel?> GetConfigAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await _mobileAuthService.GetValidAccessTokenAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        using var client = _httpClientFactory.CreateClient("PerformIqApi");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/mobile/background-sync/config");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<BackgroundSyncConfigResponseModel>(cancellationToken: cancellationToken);
    }
}
