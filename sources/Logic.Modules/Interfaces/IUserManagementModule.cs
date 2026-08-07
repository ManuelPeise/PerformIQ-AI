using Shared.Models.UserManagement;

namespace Logic.Modules.Interfaces
{
    public interface IUserManagementModule
    {
        Task<UserListResultModel> ListUsersAsync(UserListQueryModel queryModel, CancellationToken cancellationToken = default);
        Task<UserDetailsModel> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserDetailsModel> SetUserActiveStateAsync(int userId, SetUserActiveStateRequestModel requestModel, CancellationToken cancellationToken = default);
        Task<UserDetailsModel> SetUserRolesAsync(int userId, SetUserRolesRequestModel requestModel, CancellationToken cancellationToken = default);
    }
}
