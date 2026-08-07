namespace Shared.Models.Profile;

public sealed class UpdateUserProfileRequestModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirthUtc { get; set; }
    public UpdateUserAddressRequestModel? Address { get; set; }
}
