using Shared.Models.Authentication;

namespace Logic.Authentication.Interfaces
{
    public interface ICurrentUserService
    {
        Task<CurrentUserModel?> GetCurrentUser();
    }
}
