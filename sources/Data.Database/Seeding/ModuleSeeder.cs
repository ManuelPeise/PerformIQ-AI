using Data.Database.Entities.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Data.Database.Seeding
{
    public static class ModuleSeeder
    {
        public static async Task SeedDefaultModulesAsync(DatabaseContext databaseContext, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(databaseContext);

            var moduleDictionary = new Dictionary<string, string>
            {
                { DefaultModuleClaims.Dashboard, "common.captionGeneral" },
                { DefaultModuleClaims.Profile, "common.captionUser" },
            };

            var existingModuleNames = await databaseContext.Modules
                .Where(module => moduleDictionary.Keys.Contains(module.Name))
                .Select(module => module.Name)
                .ToListAsync(cancellationToken);

            var existingModuleLookup = new HashSet<string>(existingModuleNames, StringComparer.OrdinalIgnoreCase);
            var missingModuleNames = moduleDictionary.Keys.Where(moduleName => !existingModuleLookup.Contains(moduleName)).ToArray();

            if (missingModuleNames.Length == 0)
            {
                return;
            }

            foreach (var moduleName in missingModuleNames)
            {
                await databaseContext.Modules.AddAsync(new ModuleEntity { Name = moduleName, GroupResourceKey = moduleDictionary[moduleName], IsDefaultModule = true }, cancellationToken);
            }

            await databaseContext.SaveChangesAsync(cancellationToken);
        }

        public static async Task SeedProtectedModulesAsync(DatabaseContext databaseContext, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(databaseContext);

            var moduleDictionary = new Dictionary<string, string>
            {
                { ProtectedModuleClaims.UserAdministration, "common.captionAdministration" },
                { ProtectedModuleClaims.HealthConnect, "common.captionInterfaces" },
            };

            var existingModuleNames = await databaseContext.Modules
                .Where(module => moduleDictionary.Keys.Contains(module.Name))
                .Select(module => module.Name)
                .ToListAsync(cancellationToken);

            var existingModuleLookup = new HashSet<string>(existingModuleNames, StringComparer.OrdinalIgnoreCase);
            var missingModuleNames = moduleDictionary.Keys.Where(moduleName => !existingModuleLookup.Contains(moduleName)).ToArray();

            if (missingModuleNames.Length == 0)
            {
                return;
            }

            foreach (var moduleName in missingModuleNames)
            {
                await databaseContext.Modules.AddAsync(new ModuleEntity { Name = moduleName, GroupResourceKey = moduleDictionary[moduleName], IsDefaultModule = false }, cancellationToken);
            }

            await databaseContext.SaveChangesAsync(cancellationToken);
        }
    }
}
