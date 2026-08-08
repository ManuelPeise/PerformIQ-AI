using Shared.Models.Authentication;

namespace Web.Mobile.Services;

public interface IMobileAuthService
{
    Task<AuthResponseModel> LoginAsync(LoginRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<string?> GetValidAccessTokenAsync(CancellationToken cancellationToken = default);
    Task<bool> RestoreSessionAsync(CancellationToken cancellationToken = default);
    Task<bool> HasRefreshTokenAsync();
    Task LogoutAsync();
}
