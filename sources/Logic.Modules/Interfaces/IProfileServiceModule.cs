using Shared.Models.Profile;

namespace Logic.Modules.Interfaces
{
    public interface IProfileServiceModule
    {
        Task<UserProfileModel> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserProfileModel> UpsertByUserIdAsync(int userId, UpdateUserProfileRequestModel requestModel, CancellationToken cancellationToken = default);
    }
}
