namespace Shared.Models.Profile;

public sealed class UserProfileModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirthUtc { get; set; }
    public UserAddressModel? Address { get; set; }
}
