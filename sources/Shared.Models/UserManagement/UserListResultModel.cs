namespace Shared.Models.UserManagement;

public sealed class UserListResultModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<UserSummaryModel> Users { get; set; } = new();
}
