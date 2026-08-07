namespace Data.Database.Entities.User
{
    public class UserProfileEntity: AEntityBase
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public UserEntity User { get; set; } = null!;
        public UserAddressEntity? Address { get; set; }
    }
}
