using Data.Accessor.DI;
using Data.Database;
using Logic.Authentication.DI;
using Logic.Modules.DI;
using Logic.Shared.Authentication;
using Logic.Shared.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Models.Authentication;
using Shared.Models.Seeding;
using System.Text;

namespace Web.Api.Bundels
{
    internal static class ServiceRegistration
    {
        private const string CorsPolicyName = "AllowAll";

        internal static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.Configure<SystemAdminSeedOptions>(builder.Configuration.GetSection(SystemAdminSeedOptions.SectionName));
            builder.Logging.AddSeq(builder.Configuration.GetSection("Seq"));

            builder.Services.AddControllers();

            builder.Services.RegisterSwagger();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            RegisterDbServices(builder);
            AddJwtAuthenticationAndAuthorization(builder.Services, builder.Configuration);

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("is-authenticated", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(
                        UserRoleClaims.Admin,
                        UserRoleClaims.User);
                });
            });

            DataAccessorServiceRegistration.RegisterServices(builder.Services);
            LogicSharedServiceRegistration.RegisterServices(builder.Services);
            AuthenticationServiceRegistration.RegisterServices(builder.Services);
            ModuleServiceRegistration.RegisterServices(builder.Services);
        }

        private static void RegisterDbServices(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("PerformIqDb") ??
              throw new InvalidOperationException("Connection string 'PerformIqDb' not found.");

            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseMySQL(connectionString);
            });
        }

        private static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
                {
                    Title = "PerformIQ API",
                    Version = "v1"
                });
            });

        }

        private static void AddJwtAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("JWT configuration section is missing.");

            if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey) || jwtOptions.SigningKey.Length < 32)
            {
                throw new InvalidOperationException("JWT signing key must be at least 32 characters long.");
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });
        }

        private static bool HasModuleScopeOrAdmin(AuthorizationHandlerContext context, string requiredScope)
        {
            return context.User.IsInRole("Admin")
                   || context.User.Claims.Any(claim =>
                       claim.Type == AuthClaimTypes.ModuleScope
                       && string.Equals(claim.Value, requiredScope, StringComparison.OrdinalIgnoreCase));
        }
    }
}
