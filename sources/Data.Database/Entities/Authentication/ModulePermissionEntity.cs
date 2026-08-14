using Data.Database.Entities.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.Authentication;

public class ModulePermissionEntity : AEntityBase
{
    public int ModuleId { get; set; }
    public int UserId { get; set; }
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanCreate { get; set; }
    public bool CanDelete { get; set; }

    [ForeignKey(nameof(ModuleId))]
    public ModuleEntity Module { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public UserEntity User { get; set; } = null!;
}
