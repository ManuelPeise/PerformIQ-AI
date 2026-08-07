namespace Data.Database.Entities.Authentication;

public class RoleEntity : AEntityBase
{
    public string Name { get; set; } = string.Empty;

    public List<UserRoleEntity> UserRoles { get; set; } = new();
}
