using Logic.Modules.Interfaces;
using Logic.Modules.ProfileService;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Modules.DI
{
    public static class ModuleServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IProfileServiceModule, ProfileServiceModule>();
        }
    }
}
