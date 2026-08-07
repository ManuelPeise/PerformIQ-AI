using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;

namespace Data.Database.Entities;

public class UserModulePermissionEntity : AEntityBase
{
    public int UserId { get; set; }
    public int ModulePermissionId { get; set; }

    public UserEntity User { get; set; } = null!;
    public ModulePermissionEntity ModulePermission { get; set; } = null!;
}
