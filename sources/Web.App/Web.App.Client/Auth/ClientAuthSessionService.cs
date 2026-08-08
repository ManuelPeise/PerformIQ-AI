using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Shared.Models.Authentication;

namespace Web.App.Client.Auth;

public sealed class ClientAuthSessionService
{
    private static readonly ClaimsPrincipal _anonymousPrincipal = new(new ClaimsIdentity());
    private readonly JwtSecurityTokenHandler _jwtTokenHandler = new();

    private string? _accessToken;
    private DateTime _accessTokenExpiresAtUtc;
    private ClaimsPrincipal _currentPrincipal = _anonymousPrincipal;

    public event Action? SessionChanged;

    public ClaimsPrincipal CurrentPrincipal => _currentPrincipal;
    public bool IsAuthenticated => _currentPrincipal.Identity?.IsAuthenticated == true;

    public string? GetValidAccessToken()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            return null;
        }

        if (_accessTokenExpiresAtUtc <= DateTime.UtcNow.AddSeconds(15))
        {
            return null;
        }

        return _accessToken;
    }

    public void SetSession(AuthSessionResponseModel response)
    {
        _accessToken = response.AccessToken;
        _accessTokenExpiresAtUtc = response.AccessTokenExpiresAtUtc;

        var jwt = _jwtTokenHandler.ReadJwtToken(response.AccessToken);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        _currentPrincipal = new ClaimsPrincipal(identity);

        SessionChanged?.Invoke();
    }

    public void ClearSession()
    {
        _accessToken = null;
        _accessTokenExpiresAtUtc = DateTime.MinValue;
        _currentPrincipal = _anonymousPrincipal;
        SessionChanged?.Invoke();
    }
}
