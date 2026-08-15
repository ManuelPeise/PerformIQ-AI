namespace Data.Database.Entities.User;

public class UserAddressEntity : AEntityBase
{
    public int UserProfileId { get; set; }
    public string Street { get; set; } = null!;
    public string HouseNumber { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string StateOrProvince { get; set; } = null!;
    public string CountryCode { get; set; } = null!;

    public UserProfileEntity UserProfile { get; set; } = null!;
}
