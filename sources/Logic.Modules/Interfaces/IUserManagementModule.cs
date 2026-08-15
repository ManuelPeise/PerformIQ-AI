using Shared.Models.UserManagement;

namespace Logic.Modules.Interfaces
{
    public interface IUserManagementModule
    {
        Task<IEnumerable<UserDataExportModel>> ListUsersAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateUserAsync(UserDataExportModel user);
        Task DeleteUsersAsync();
    }
}
