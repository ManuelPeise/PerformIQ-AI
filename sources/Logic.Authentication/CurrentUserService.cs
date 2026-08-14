using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database.Entities;
using Data.Database.Entities.Authentication;
using Logic.Authentication.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Models.Authentication;
using System.Linq.Expressions;

namespace Logic.Authentication
{
    public class CurrentUserService : LogicBase, ICurrentUserService
    {
        private Func<int, UserRoleEntity>? getUserByIdFunc;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(
            ILogger<CurrentUserService> logger, 
            IApplicationUnitOfWork applicationUnitOfWork, 
            IHttpContextAccessor httpContextAccessor)
            : base(applicationUnitOfWork, httpContextAccessor) 
        {
            _logger = logger;
        }

        public async Task<CurrentUserModel?> GetCurrentUser()
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (!currentUserId.HasValue)
                {
                   throw new InvalidOperationException("Current user ID is not available.");
                }

                var currentUser = await ApplicationUnitOfWork.Users.GetByIdAsync(currentUserId.Value);
                
                var roles = await ApplicationUnitOfWork.UserRoles.GetAsync(new DbQueryOptions<UserRoleEntity> 
                        { 
                            WhereExpression = ur => ur.UserId == currentUserId.Value,
                            Includes = new List<Expression<Func<UserRoleEntity, object>>>
                            {
                                ur => ur.Role
                            }
                });

                var permissions = await ApplicationUnitOfWork.ModulePermissions.GetAsync(new DbQueryOptions<ModulePermissionEntity>
                {
                    WhereExpression = up => up.UserId == currentUserId.Value,
                    Includes = new List<Expression<Func<ModulePermissionEntity, object>>>
                    {
                        up => up.Module
                    }
                });

                if (currentUser != null && roles != null && permissions != null)
                {
                    return new CurrentUserModel
                    {
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName,
                        Email = currentUser.Email,
                        Roles = roles.Select(r => r.Role.Name).ToList(),
                        Permissions = permissions.Select(p => new Permission
                        {
                            Module = p.Module.Name,
                            CanView = p.CanView,
                            CanCreate = p.CanCreate,
                            CanEdit = p.CanEdit,
                            CanDelete = p.CanDelete
                        }).ToList()
                    };
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while retrieving the current user.");
            }
            return null;
        }
    }
}
