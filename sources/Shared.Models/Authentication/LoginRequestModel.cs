namespace Shared.Models.Authentication;

public sealed class LoginRequestModel
{
    public string UserNameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
