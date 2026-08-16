using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database.Entities;
using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;
using Logic.Modules.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Models.Authentication;
using Shared.Models.UserManagement;
using System.Linq.Expressions;

namespace Logic.Modules.UserManagement;

public class UserManagementModule : LogicBase, IUserManagementModule
{
    private readonly ILogger<UserManagementModule> _logger;
    private const int ThirtyDaysAgoInDays = -30;
    public UserManagementModule(
        ILogger<UserManagementModule> logger,
        IHttpContextAccessor httpContextAccessor,
        IApplicationUnitOfWork applicationUnitOfWork) : base(applicationUnitOfWork, httpContextAccessor)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<UserDataExportModel>> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var userEntities = await ApplicationUnitOfWork.Users.GetAsync(
                new DbQueryOptions<UserEntity>
                {
                    AsNoTracking = false,
                    Includes = {
                    x => x.Profile
                    },
                    OrderByExpression = x => x.UserName,
                },
                cancellationToken);

            await EnsureModulesLoaded(userEntities, cancellationToken);
            await EnsureRolesLoaded(userEntities, cancellationToken);
           
            return MapToExportModels(userEntities);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while listing users.");

            return new List<UserDataExportModel>();
        }
    }

    public async Task<bool> UpdateUserAsync(UserDataExportModel user)
    {
        try
        {
            ValidateUserModel(user);

            var userEntity = await ApplicationUnitOfWork.Users.GetByIdAsync(
                user.UserId,
                asNoTracking: false,
                includeExpressions: new List<Expression<Func<UserEntity, object>>> { x => x.Profile, x => x.UserRoles, x => x.ModulePermissions });

            if (userEntity == null)
            {
                throw new InvalidOperationException($"User with ID {user.UserId} not found.");
            }

            if (user.IsMarkedAsDeleted)
            {
                userEntity.IsActive = false;
                userEntity.IsMarkedAsDeleted = true;
                userEntity.IsMarkedAdDeletedBy = GetCurrentUserName();
                userEntity.IsMarkedAdDeletedAt = DateTime.UtcNow.ToString("o");
            }
            else
            {
                userEntity.IsActive = user.IsActive;
                userEntity.IsMarkedAsDeleted = false;
                userEntity.IsMarkedAdDeletedBy = null;
                userEntity.IsMarkedAdDeletedAt = null;

                await MapUserRoles(userEntity, user.Roles);

                await MapUserModulePermissions(userEntity, user.Permissions);
            }

            await ApplicationUnitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while updating user with ID {UserId}.", user.UserId);

            return false;
        }
    }

    public async Task DeleteUsersAsync()
    {
        try
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(ThirtyDaysAgoInDays);

            var userEntitiesToDelete = await ApplicationUnitOfWork.Users.GetAsync(new DbQueryOptions<UserEntity>
            {
                AsNoTracking = false,
                WhereExpression = x => x.IsMarkedAsDeleted && x.IsMarkedAdDeletedAt != null && DateTime.Parse(x.IsMarkedAdDeletedAt) >= thirtyDaysAgo,
            });

            if (userEntitiesToDelete.Any())
            {
                await ApplicationUnitOfWork.Users.DeleteRange(userEntitiesToDelete);
                await ApplicationUnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Deleted {Count} users with ids [{UserIds}].", userEntitiesToDelete.Count(), string.Join(", ", userEntitiesToDelete.Select(u => u.Id)));
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while deleting users.");
        }
    }

    private void ValidateUserModel(UserDataExportModel user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
    }

    private async Task EnsureRolesLoaded(IReadOnlyList<UserEntity> userEntities, CancellationToken cancellationToken)
    {
        foreach (var userEntity in userEntities)
        {
            await ApplicationUnitOfWork.UserRoles.GetAsync(
                new DbQueryOptions<UserRoleEntity>
                {
                    WhereExpression = x => x.UserId == userEntity.Id,
                    Includes = { x => x.Role }
                }, cancellationToken: cancellationToken);
        }
    }

    private async Task EnsureModulesLoaded(IReadOnlyList<UserEntity> userEntities, CancellationToken cancellationToken)
    {
        foreach (var userEntity in userEntities)
        {
            await ApplicationUnitOfWork.ModulePermissions.GetAsync(
                new DbQueryOptions<ModulePermissionEntity>
                {
                    WhereExpression = x => x.UserId == userEntity.Id,
                    Includes = { x => x.Module }
                }, cancellationToken: cancellationToken);
        }
    }

    private IEnumerable<UserDataExportModel> MapToExportModels(IEnumerable<UserEntity> users)
    {
        return (from user in users
                select new UserDataExportModel
                {
                    UserId = user.Id,
                    FirstName = user.Profile?.FirstName,
                    LastName = user.Profile?.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    DateOfBirthUtc = user.Profile is null ? null : user.Profile.DateOfBirth?.ToString("o"),
                    IsActive = user.IsActive,
                    Roles = user.UserRoles
                        .Select(x => x.Role.Name)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList(),
                    Permissions = user.ModulePermissions
                        .Select(x => new Permission
                        {
                            Module = x.Module.Name,
                            GroupResourceKey = x.Module.GroupResourceKey,
                            CanView = x.CanView,
                            CanEdit = x.CanEdit,
                            CanCreate = x.CanCreate,
                            CanDelete = x.CanDelete,
                        })
                        .ToList()
                });

    }

    private async Task MapUserRoles(UserEntity userEntity, List<string> roles)
    {
        var userRoles = await ApplicationUnitOfWork.Roles.GetAsync();

        var userRolesToDelete = userEntity.UserRoles
            .Where(x => !roles.Contains(x.Role.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (userRolesToDelete.Any())
        {
            await ApplicationUnitOfWork.UserRoles.DeleteRange(userRolesToDelete);
        }

        var userRolesToAdd = userRoles
            .Where(x => roles.Contains(x.Name, StringComparer.OrdinalIgnoreCase) && !userEntity.UserRoles.Any(y => y.RoleId == x.Id))
            .ToList();

        if (userRolesToAdd.Any())
        {
            foreach (var role in userRolesToAdd)
            {
                var userRoleEntity = new UserRoleEntity
                {
                    UserId = userEntity.Id,
                    RoleId = role.Id
                };

                await ApplicationUnitOfWork.UserRoles.AddAsync(userRoleEntity);
            }
        }

    }

    private async Task MapUserModulePermissions(UserEntity userEntity, List<Permission> permissions)
    {
        foreach (var permissionEntity in userEntity.ModulePermissions)
        {
            await ApplicationUnitOfWork.Modules.GetByIdAsync(permissionEntity.ModuleId);

            var currentPermission = permissions.FirstOrDefault(p => p.Module.Equals(permissionEntity.Module.Name, StringComparison.OrdinalIgnoreCase));

            if (currentPermission == null)
            {
                continue;
            }

            permissionEntity.CanView = currentPermission.CanView;
            permissionEntity.CanCreate = currentPermission.CanCreate;
            permissionEntity.CanEdit = currentPermission.CanEdit;
            permissionEntity.CanDelete = currentPermission.CanDelete;
        }
    }
}
