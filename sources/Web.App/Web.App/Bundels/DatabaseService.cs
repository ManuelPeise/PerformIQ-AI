using Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.App.Bundels
{
    public static class DatabaseService
    {
        public static void ConfigureDatabaseService(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("PerformIqDb") ??
                throw new InvalidOperationException("Connection string 'PerformIqDb' not found.");

            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseMySQL(connectionString);
            });
        }
    }
}
