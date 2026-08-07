using Shared.Models.Authentication;

namespace Logic.Authentication.Interfaces;

public interface IUserAuthenticationService
{
    Task<AuthResponseModel> RegisterAsync(RegisterRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<AuthResponseModel> LoginAsync(LoginRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<AuthResponseModel> RefreshAsync(RefreshTokenRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<bool> RevokeAsync(RevokeTokenRequestModel requestModel, CancellationToken cancellationToken = default);
}
