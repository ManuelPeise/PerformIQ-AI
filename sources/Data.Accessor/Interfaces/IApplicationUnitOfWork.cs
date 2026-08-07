using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;
using Data.Database.Entities;

namespace Data.Accessor.Interfaces;

public interface IApplicationUnitOfWork
{
    IRepositoryBase<UserEntity> Users { get; }
    IRepositoryBase<UserProfileEntity> UserProfiles { get; }
    IRepositoryBase<UserAddressEntity> UserAddresses { get; }
    IRepositoryBase<UserCredentialsEntity> UserCredentials { get; }
    IRepositoryBase<RoleEntity> Roles { get; }
    IRepositoryBase<Data.Database.Entities.UserRoleEntity> UserRoles { get; }
    IRepositoryBase<ModulePermissionEntity> ModulePermissions { get; }
    IRepositoryBase<Data.Database.Entities.UserModulePermissionEntity> UserModulePermissions { get; }
    IRepositoryBase<RefreshTokenEntity> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
