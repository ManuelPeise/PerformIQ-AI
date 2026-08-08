using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shared.Models.Authentication;

namespace Web.App.Client.Auth;

public sealed class AuthHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ClientAuthSessionService _authSessionService;
    private readonly IClientAuthenticationService _authenticationService;

    public AuthHttpClient(
        HttpClient httpClient,
        ClientAuthSessionService authSessionService,
        IClientAuthenticationService authenticationService)
    {
        _httpClient = httpClient;
        _authSessionService = authSessionService;
        _authenticationService = authenticationService;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        var response = await SendWithAccessTokenAsync(requestMessage, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        response.Dispose();
        var refreshed = await _authenticationService.TryRefreshAsync(cancellationToken);
        if (!refreshed)
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = requestMessage };
        }

        var retryRequest = await CloneRequestAsync(requestMessage, cancellationToken);
        return await SendWithAccessTokenAsync(retryRequest, cancellationToken);
    }

    public async Task<T?> GetFromJsonAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    private async Task<HttpResponseMessage> SendWithAccessTokenAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        var token = _authSessionService.GetValidAccessToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await _httpClient.SendAsync(requestMessage, cancellationToken);
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        var clone = CreateBaseClone(requestMessage);
        CopyRequestHeaders(requestMessage, clone);
        clone.Content = await CloneContentAsync(requestMessage, cancellationToken);
        return clone;
    }

    private static HttpRequestMessage CreateBaseClone(HttpRequestMessage requestMessage)
    {
        return new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri)
        {
            Version = requestMessage.Version,
            VersionPolicy = requestMessage.VersionPolicy
        };
    }

    private static void CopyRequestHeaders(HttpRequestMessage source, HttpRequestMessage destination)
    {
        foreach (var header in source.Headers)
        {
            destination.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    private static async Task<HttpContent?> CloneContentAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        if (requestMessage.Content is null)
        {
            return null;
        }

        var bytes = await requestMessage.Content.ReadAsByteArrayAsync(cancellationToken);
        var copiedContent = new ByteArrayContent(bytes);
        foreach (var header in requestMessage.Content.Headers)
        {
            copiedContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return copiedContent;
    }
}
