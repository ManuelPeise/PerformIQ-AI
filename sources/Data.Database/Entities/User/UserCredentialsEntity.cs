using Data.Database.Entities.Authentication;

namespace Data.Database.Entities.User;

public class UserCredentialsEntity : AEntityBase
{
    public int UserId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public int FaildLoginAttemts { get; set; }

    public UserEntity User { get; set; } = null!;
    public List<RefreshTokenEntity> RefreshTokens { get; set; } = new();
}
