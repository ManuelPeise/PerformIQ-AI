namespace Data.Database.Entities.Authentication;

public class ModulePermissionEntity : AEntityBase
{
    public string Module { get; set; } = string.Empty;
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanCreate { get; set; }
    public bool CanDelete { get; set; }

    public List<UserModulePermissionEntity> UserModulePermissions { get; set; } = new();
}
