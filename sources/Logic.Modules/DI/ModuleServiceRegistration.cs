using Logic.Modules.Interfaces;
using Logic.Modules.ProfileService;
using Logic.Modules.UserManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Modules.DI
{
    public static class ModuleServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IProfileServiceModule, ProfileServiceModule>();
            services.AddScoped<IUserManagementModule, UserManagementModule>();
        }
    }
}
