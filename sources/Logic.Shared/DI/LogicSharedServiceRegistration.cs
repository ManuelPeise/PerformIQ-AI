using Logic.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Shared.DI
{
    public static class LogicSharedServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services)
        {
           services.AddScoped<ILogicBase, LogicBase>();
        }
    }
}
