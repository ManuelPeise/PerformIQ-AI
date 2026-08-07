namespace Shared.Models.Authentication;

public sealed class RefreshTokenRequestModel
{
    public string RefreshToken { get; set; } = string.Empty;
}
