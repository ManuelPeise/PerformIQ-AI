namespace Data.Database.Entities.User;

public class UserAddressEntity : AEntityBase
{
    public int UserProfileId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? StateOrProvince { get; set; }
    public string CountryCode { get; set; } = string.Empty;

    public UserProfileEntity UserProfile { get; set; } = null!;
}
