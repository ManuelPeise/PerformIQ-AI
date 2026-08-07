using Data.Database;
using Data.Database.Seeding;
using Microsoft.EntityFrameworkCore;

namespace Web.App.Bundels;

internal static class DatabaseInitialization
{
    public static async Task InitializeDatabaseAsync(this WebApplication webApplication, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        await using var scope = webApplication.Services.CreateAsyncScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var hasMigrations = databaseContext.Database.GetMigrations().Any();
        
        if (hasMigrations)
        {
            await databaseContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await databaseContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        await RoleSeeder.SeedRolesAsync(databaseContext, cancellationToken);
    }
}
