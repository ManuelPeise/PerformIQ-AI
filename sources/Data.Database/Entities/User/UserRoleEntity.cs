using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;

namespace Data.Database.Entities;

public class UserRoleEntity : AEntityBase
{
    public int UserId { get; set; }
    public int RoleId { get; set; }

    public UserEntity User { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
}
