using Data.Accessor.Interfaces;
using Data.Accessor.Repositories;
using Data.Accessor.UnitsOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Accessor.DI
{
    public static class DataAccessorServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();
            services.AddScoped<IUnitOfWorkBase, ApplicationUnitOfWork>();
        }
    }
}
