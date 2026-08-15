using Data.Database.Entities;
using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Seeding;

namespace Data.Database.Seeding;

public static class SystemAdminSeeder
{
    public static async Task SeedSystemAdminAsync(
        DatabaseContext databaseContext,
        SystemAdminSeedOptions options,
        string passwordHash,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(databaseContext);
        ArgumentNullException.ThrowIfNull(options);

        if (!options.Enabled)
        {
            return;
        }
        var normalizedUserName = options.UserName.Trim();
        var normalizedEmail = options.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalizedUserName))
        {
            throw new InvalidOperationException("SystemAdminSeed.UserName is required.");
        }

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            throw new InvalidOperationException("SystemAdminSeed.Email is required.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new InvalidOperationException("A hashed password is required for seeding the system admin user.");
        }

        EnsurePasswordHashFormat(passwordHash);

        var hasExistingUser = await databaseContext.Users.AnyAsync(
            user => user.Email.ToLower() == normalizedEmail || user.UserName.ToLower() == normalizedUserName.ToLower(),
            cancellationToken);
        if (hasExistingUser)
        {
            return;
        }

        var adminRole = await databaseContext.Roles
            .FirstOrDefaultAsync(role => role.Name == UserRoleClaims.Admin, cancellationToken);

        if (adminRole is null)
        {
            throw new InvalidOperationException("Required role 'Admin' was not found. Ensure role seeding has run.");
        }

        var userEntity = new UserEntity
        {
            UserName = normalizedUserName,
            Email = normalizedEmail,
            IsActive = options.IsActive,
            Profile = new UserProfileEntity
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                DateOfBirth = null,
                Address = new UserAddressEntity
                {
                    Street = string.Empty,
                    HouseNumber = string.Empty,
                    PostalCode = string.Empty,
                    City = string.Empty,
                    StateOrProvince = string.Empty,
                    CountryCode = string.Empty
                }
            },
            Credentials = new UserCredentialsEntity
            {
                PasswordHash = passwordHash,
                FaildLoginAttemts = 0
            }
        };
        userEntity.UserRoles.Add(new UserRoleEntity { RoleId = adminRole.Id });

        var grantedModulesEntities = await databaseContext.Modules
               .ToListAsync(cancellationToken);

        var userModulePermissions = grantedModulesEntities.Select(module => new ModulePermissionEntity
        {
            ModuleId = module.Id,
            CanView = true,
            CanCreate = true,
            CanEdit = true,
            CanDelete = false,
        }).ToList();

        userEntity.ModulePermissions = userModulePermissions;

        await databaseContext.Users.AddAsync(userEntity, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);
    }

    private static void EnsurePasswordHashFormat(string passwordHash)
    {
        var parts = passwordHash.Split(':');
        if (parts.Length != 3 || !int.TryParse(parts[0], out _))
        {
            throw new InvalidOperationException("SystemAdminSeed.PasswordHash format is invalid.");
        }

        try
        {
            _ = Convert.FromBase64String(parts[1]);
            _ = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("SystemAdminSeed.PasswordHash format is invalid.", exception);
        }
    }
}
