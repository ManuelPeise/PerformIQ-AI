using Logic.Authentication.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Authentication.DI
{
    public static class AuthenticationServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHashService, PasswordHashService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}
