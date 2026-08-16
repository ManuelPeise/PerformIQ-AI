using Logic.Modules.Interfaces;
using Shared.Models.UserManagement;

namespace Web.Api.Services.ApiControllers.UserService
{
    internal static class UserServiceEndpointMapper
    {
        internal static void MapUserServiceEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            var group = endpointRouteBuilder.MapGroup("/api/administration/userservice").WithTags("User Service");

            group.MapGet("/getusers", GetExportUserDataAsync).RequireAuthorization("user-management");
            group.MapPost("/updateuser", UpdateUserDataAsync).RequireAuthorization("user-management");
            group.MapGet("/deleteusers", DeleteUsersAsync).RequireAuthorization("user-management");

        }

        private static async Task<IEnumerable<UserDataExportModel>> GetExportUserDataAsync(
            IUserManagementModule userService,
            CancellationToken cancellationToken)
        {
            var response = await userService.ListUsersAsync(cancellationToken);
            return response;
        }

        private static async Task<bool> UpdateUserDataAsync(
            IUserManagementModule userService,
            UserDataExportModel model)
        {
            var response = await userService.UpdateUserAsync(model);
            return response;
        }

        private static async Task DeleteUsersAsync(
           IUserManagementModule userService)
        {
            await userService.DeleteUsersAsync();

        }
    }
}
