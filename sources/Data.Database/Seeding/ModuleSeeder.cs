using Data.Database.Entities.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Data.Database.Seeding
{
    public static class ModuleSeeder
    {
        public static async Task SeedDefaultModulesAsync(DatabaseContext databaseContext, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(databaseContext);

            var moduleNames = new[]
            {
                DefaultModuleClaims.Dashboard,
                DefaultModuleClaims.Profile,
            };

            var existingModuleNames = await databaseContext.Modules
                .Where(module => moduleNames.Contains(module.Name))
                .Select(module => module.Name)
                .ToListAsync(cancellationToken);

            var existingModuleLookup = new HashSet<string>(existingModuleNames, StringComparer.OrdinalIgnoreCase);
            var missingModuleNames = moduleNames.Where(moduleName => !existingModuleLookup.Contains(moduleName)).ToArray();

            if (missingModuleNames.Length == 0)
            {
                return;
            }

            foreach (var moduleName in missingModuleNames)
            {
                await databaseContext.Modules.AddAsync(new ModuleEntity { Name = moduleName, IsDefaultModule = true }, cancellationToken);
            }

            await databaseContext.SaveChangesAsync(cancellationToken);
        }

        public static async Task SeedProtectedModulesAsync(DatabaseContext databaseContext, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(databaseContext);

            var moduleNames = new[]
            {
                ProtectedModuleClaims.UserAdministration,
                ProtectedModuleClaims.HealthConnect,
            };

            var existingModuleNames = await databaseContext.Modules
                .Where(module => moduleNames.Contains(module.Name))
                .Select(module => module.Name)
                .ToListAsync(cancellationToken);

            var existingModuleLookup = new HashSet<string>(existingModuleNames, StringComparer.OrdinalIgnoreCase);
            var missingModuleNames = moduleNames.Where(moduleName => !existingModuleLookup.Contains(moduleName)).ToArray();

            if (missingModuleNames.Length == 0)
            {
                return;
            }

            foreach (var moduleName in missingModuleNames)
            {
                await databaseContext.Modules.AddAsync(new ModuleEntity { Name = moduleName, IsDefaultModule = false }, cancellationToken);
            }

            await databaseContext.SaveChangesAsync(cancellationToken);
        }
    }
}
