using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shared.Models.Mobile;

namespace Web.Mobile.Services;

public sealed class BackgroundSyncSender : IBackgroundSyncSender
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMobileAuthService _mobileAuthService;

    public BackgroundSyncSender(IHttpClientFactory httpClientFactory, IMobileAuthService mobileAuthService)
    {
        _httpClientFactory = httpClientFactory;
        _mobileAuthService = mobileAuthService;
    }

    public async Task<bool> SendAsync(BackgroundSyncDataRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var accessToken = await _mobileAuthService.GetValidAccessTokenAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return false;
        }

        using var client = _httpClientFactory.CreateClient("PerformIqApi");
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/mobile/background-sync/data")
        {
            Content = JsonContent.Create(requestModel)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await client.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
