namespace Shared.Models.Authentication;

public sealed class AuthSessionResponseModel
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAtUtc { get; set; }
}
