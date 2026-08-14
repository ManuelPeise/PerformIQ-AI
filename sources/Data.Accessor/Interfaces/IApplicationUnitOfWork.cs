using Data.Database.Entities;
using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;

namespace Data.Accessor.Interfaces;

public interface IApplicationUnitOfWork
{
    IRepositoryBase<UserEntity> Users { get; }
    IRepositoryBase<UserProfileEntity> UserProfiles { get; }
    IRepositoryBase<UserAddressEntity> UserAddresses { get; }
    IRepositoryBase<UserCredentialsEntity> UserCredentials { get; }
    IRepositoryBase<RoleEntity> Roles { get; }
    IRepositoryBase<UserRoleEntity> UserRoles { get; }
    IRepositoryBase<ModulePermissionEntity> ModulePermissions { get; }
    IRepositoryBase<ModuleEntity> Modules { get; }
    IRepositoryBase<RefreshTokenEntity> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
