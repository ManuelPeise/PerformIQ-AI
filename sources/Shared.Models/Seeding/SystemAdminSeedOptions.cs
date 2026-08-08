namespace Shared.Models.Seeding;

public sealed class SystemAdminSeedOptions
{
    public const string SectionName = "SystemAdminSeed";

    public bool Enabled { get; set; } = true;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Password { get; set; } = string.Empty;
}
