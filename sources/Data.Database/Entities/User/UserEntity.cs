using Data.Database.Entities.Authentication;

namespace Data.Database.Entities.User;

public class UserEntity : AEntityBase
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public UserProfileEntity Profile { get; set; } = null!;
    public UserCredentialsEntity Credentials { get; set; } = null!;
    public List<UserRoleEntity> UserRoles { get; set; } = new();
    public List<ModulePermissionEntity> ModulePermissions { get; set; } = new();
}
