namespace Shared.Models.Authentication;

public sealed class RevokeTokenRequestModel
{
    public string RefreshToken { get; set; } = string.Empty;
}
