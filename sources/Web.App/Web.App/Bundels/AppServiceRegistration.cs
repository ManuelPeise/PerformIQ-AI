using Data.Accessor.DI;
using Logic.Authentication.DI;
using Logic.Modules.DI;
using Logic.Shared.DI;

namespace Web.App.Bundels
{
    internal static class AppServiceRegistration
    {
        public static void AddWebAppServices(this WebApplicationBuilder builder)
        {
            DatabaseService.ConfigureDatabaseService(builder);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            builder.Services.AddSwaggerDocumentation();
            builder.Services.AddJwtAuthenticationAndAuthorization(builder.Configuration);

            DataAccessorServiceRegistration.RegisterServices(builder.Services);
            LogicSharedServiceRegistration.RegisterServices(builder.Services);
            AuthenticationServiceRegistration.RegisterServices(builder.Services);
            ModuleServiceRegistration.RegisterServices(builder.Services);
        }
    }
}
