using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Data.Accessor.UnitsOfWork
{
    public class ApplicationUnitOfWork : IApplicationUnitOfWork, IUnitOfWorkBase
    {
        private readonly DatabaseContext _context;
        private readonly HttpContext? _httpContext;
        public IRepositoryBase<UserEntity> Users { get; }
        public IRepositoryBase<UserProfileEntity> UserProfiles { get; }
        public IRepositoryBase<UserAddressEntity> UserAddresses { get; }
        public IRepositoryBase<UserCredentialsEntity> UserCredentials { get; }
        public IRepositoryBase<RoleEntity> Roles { get; }
        public IRepositoryBase<UserRoleEntity> UserRoles { get; }
        public IRepositoryBase<ModulePermissionEntity> ModulePermissions { get; }
        public IRepositoryBase<UserModulePermissionEntity> UserModulePermissions { get; }
        public IRepositoryBase<RefreshTokenEntity> RefreshTokens { get; }

        public ApplicationUnitOfWork(
            DatabaseContext context,
            IHttpContextAccessor httpContextAccessor,
            IRepositoryBase<UserEntity> users,
            IRepositoryBase<UserProfileEntity> userProfiles,
            IRepositoryBase<UserAddressEntity> userAddresses,
            IRepositoryBase<UserCredentialsEntity> userCredentials,
            IRepositoryBase<RoleEntity> roles,
            IRepositoryBase<UserRoleEntity> userRoles,
            IRepositoryBase<ModulePermissionEntity> modulePermissions,
            IRepositoryBase<UserModulePermissionEntity> userModulePermissions,
            IRepositoryBase<RefreshTokenEntity> refreshTokens)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
            Users = users;
            UserProfiles = userProfiles;
            UserAddresses = userAddresses;
            UserCredentials = userCredentials;
            Roles = roles;
            UserRoles = userRoles;
            ModulePermissions = modulePermissions;
            UserModulePermissions = userModulePermissions;
            RefreshTokens = refreshTokens;
        }

        public async Task<int> SaveChangesAsync(
             CancellationToken cancellationToken = default)
        {
            if (_context == null) throw new ObjectDisposedException(nameof(ApplicationUnitOfWork));

            var now = DateTime.UtcNow;
            var userName = _httpContext?.User?.Identity?.Name ?? "System";
            var entries = _context.ChangeTracker.Entries<AEntityBase>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userName;

                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(AEntityBase.CreatedAt)).IsModified = false;
                    entry.Property(nameof(AEntityBase.CreatedBy)).IsModified = false;

                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userName;
                }
            }

            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
