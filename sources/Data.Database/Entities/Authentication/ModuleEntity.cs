namespace Data.Database.Entities.Authentication
{
    public class ModuleEntity: AEntityBase
    {
        public string Name { get; set; } = null!;
        public string GroupResourceKey { get; set; } = null!;
        public bool IsDefaultModule { get; set; }

        public List<ModulePermissionEntity> ModulePermissions { get; set; } = [];
    }
}
