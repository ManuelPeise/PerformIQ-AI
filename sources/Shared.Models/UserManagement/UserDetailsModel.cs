using Shared.Models.Profile;

namespace Shared.Models.UserManagement;

public sealed class UserDetailsModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirthUtc { get; set; }
    public UserAddressModel? Address { get; set; }
    public List<string> Roles { get; set; } = new();
}
