using Data.Accessor.DI;
using Logic.Authentication.DI;
using Logic.Modules.DI;
using Logic.Shared.DI;
using Microsoft.AspNetCore.Components;
using Shared.Models.Seeding;
using Web.App.Client.Auth;
using Web.App.Client.Pages.Auth.ViewModels;
using Web.App.Client.Pages.ViewModels;

namespace Web.App.Bundels
{
    internal static class AppServiceRegistration
    {
        public static void AddWebAppServices(this WebApplicationBuilder builder)
        {
            DatabaseService.ConfigureDatabaseService(builder);
            builder.Services.Configure<SystemAdminSeedOptions>(builder.Configuration.GetSection(SystemAdminSeedOptions.SectionName));

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            builder.Services.AddSwaggerDocumentation();
            builder.Services.AddJwtAuthenticationAndAuthorization(builder.Configuration);
            builder.Services.AddScoped(sp =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
            });
            builder.Services.AddScoped<ClientAuthSessionService>();
            builder.Services.AddScoped<IClientAuthenticationService, ClientAuthenticationService>();
            builder.Services.AddScoped<AuthHttpClient>();
            builder.Services.AddScoped<LoginViewModel>();
            builder.Services.AddScoped<RegisterViewModel>();
            builder.Services.AddScoped<HomeViewModel>();

            DataAccessorServiceRegistration.RegisterServices(builder.Services);
            LogicSharedServiceRegistration.RegisterServices(builder.Services);
            AuthenticationServiceRegistration.RegisterServices(builder.Services);
            ModuleServiceRegistration.RegisterServices(builder.Services);
        }
    }
}
