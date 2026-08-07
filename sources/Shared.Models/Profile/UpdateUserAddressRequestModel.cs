namespace Shared.Models.Profile;

public sealed class UpdateUserAddressRequestModel
{
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? StateOrProvince { get; set; }
    public string? CountryCode { get; set; }
}
