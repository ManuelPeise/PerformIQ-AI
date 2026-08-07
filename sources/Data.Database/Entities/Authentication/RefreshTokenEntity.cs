using Data.Database.Entities.User;

namespace Data.Database.Entities.Authentication;

public class RefreshTokenEntity : AEntityBase
{
    public int UserCredentialsId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
    public string? RevokedByIp { get; set; }
    public string? ReplacedByTokenHash { get; set; }

    public UserCredentialsEntity UserCredentials { get; set; } = null!;
}
