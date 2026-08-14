using Data.Database;
using Data.Database.Seeding;
using Logic.Authentication.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Models.Seeding;

namespace Web.Api.Bundels
{
    public static class DatabaseInitialization
    {
        public static async Task InitializeDatabaseAsync(this WebApplication webApplication, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(webApplication);

            await using var scope = webApplication.Services.CreateAsyncScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var useEnsureCreated = webApplication.Configuration.GetValue<bool>("DatabaseInitialization:UseEnsureCreated");

            if (useEnsureCreated)
            {
                await databaseContext.Database.EnsureCreatedAsync(cancellationToken);
            }
            else
            {
                var hasMigrations = databaseContext.Database.GetMigrations().Any();
                if (hasMigrations)
                {
                    await databaseContext.Database.MigrateAsync(cancellationToken);
                }
                else
                {
                    await databaseContext.Database.EnsureCreatedAsync(cancellationToken);
                }
            }

            await ModuleSeeder.SeedDefaultModulesAsync(databaseContext, cancellationToken);
            await ModuleSeeder.SeedProtectedModulesAsync(databaseContext, cancellationToken);

            await RoleSeeder.SeedRolesAsync(databaseContext, cancellationToken);

            var systemAdminSeedOptions = scope.ServiceProvider
                .GetRequiredService<IOptions<SystemAdminSeedOptions>>()
                .Value;

            var normalizedPassword = systemAdminSeedOptions.Password.Trim();
            if (string.IsNullOrWhiteSpace(normalizedPassword))
            {
                throw new InvalidOperationException("SystemAdminSeed.Password is required.");
            }

            var passwordHashService = scope.ServiceProvider.GetRequiredService<IPasswordHashService>();
            var hashedPassword = passwordHashService.HashPassword(normalizedPassword);
            
            await SystemAdminSeeder.SeedSystemAdminAsync(databaseContext, systemAdminSeedOptions, hashedPassword, cancellationToken);
        }
    }
}
