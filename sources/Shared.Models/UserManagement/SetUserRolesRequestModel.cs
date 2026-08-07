namespace Shared.Models.UserManagement;

public sealed class SetUserRolesRequestModel
{
    public List<string> RoleNames { get; set; } = new();
}
