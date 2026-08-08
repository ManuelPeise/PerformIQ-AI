namespace Web.Mobile.Services;

public sealed class AuthenticationStateService : IAuthenticationStateService
{
    private bool _isAuthenticated;

    public bool IsAuthenticated => _isAuthenticated;

    public event EventHandler<bool>? AuthenticationStateChanged;

    public void SetAuthenticated(bool isAuthenticated)
    {
        if (_isAuthenticated == isAuthenticated)
        {
            return;
        }

        _isAuthenticated = isAuthenticated;
        AuthenticationStateChanged?.Invoke(this, _isAuthenticated);
    }
}
