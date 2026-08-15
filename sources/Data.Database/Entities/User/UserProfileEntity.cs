namespace Data.Database.Entities.User
{
    public class UserProfileEntity: AEntityBase
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; } = null;
        public UserEntity User { get; set; } = null!;
        public UserAddressEntity Address { get; set; } = null!;
    }
}
