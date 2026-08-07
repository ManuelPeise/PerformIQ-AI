using Data.Database.Entities.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Data.Database.Seeding;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(DatabaseContext databaseContext, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(databaseContext);

        var roleNames = new[]
        {
            UserRoleClaims.SystemAdmin,
            UserRoleClaims.Admin,
            UserRoleClaims.User,
            UserRoleClaims.Guest
        };

        var existingRoleNames = await databaseContext.Roles
            .Where(role => roleNames.Contains(role.Name))
            .Select(role => role.Name)
            .ToListAsync(cancellationToken);

        var existingRoleLookup = new HashSet<string>(existingRoleNames, StringComparer.OrdinalIgnoreCase);
        var missingRoleNames = roleNames.Where(roleName => !existingRoleLookup.Contains(roleName)).ToArray();

        if (missingRoleNames.Length == 0)
        {
            return;
        }

        foreach (var roleName in missingRoleNames)
        {
            await databaseContext.Roles.AddAsync(new RoleEntity { Name = roleName }, cancellationToken);
        }

        await databaseContext.SaveChangesAsync(cancellationToken);
    }
}
