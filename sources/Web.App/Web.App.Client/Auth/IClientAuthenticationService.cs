using Shared.Models.Authentication;

namespace Web.App.Client.Auth;

public interface IClientAuthenticationService
{
    Task<bool> InitializeAsync(CancellationToken cancellationToken = default);
    Task<AuthSessionResponseModel> LoginAsync(LoginRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<AuthSessionResponseModel> RegisterAsync(RegisterRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<bool> TryRefreshAsync(CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
}
