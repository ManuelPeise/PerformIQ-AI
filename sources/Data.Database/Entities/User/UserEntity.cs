namespace Data.Database.Entities.User;

public class UserEntity : AEntityBase
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public UserProfileEntity? Profile { get; set; }
    public UserCredentialsEntity? Credentials { get; set; }
    public List<UserRoleEntity> UserRoles { get; set; } = new();
    public List<UserModulePermissionEntity> UserModulePermissions { get; set; } = new();
}
