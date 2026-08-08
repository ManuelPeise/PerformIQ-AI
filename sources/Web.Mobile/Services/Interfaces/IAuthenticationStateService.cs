namespace Web.Mobile.Services;

public interface IAuthenticationStateService
{
    bool IsAuthenticated { get; }
    event EventHandler<bool>? AuthenticationStateChanged;
    void SetAuthenticated(bool isAuthenticated);
}
