using System.Text;
using Data.Database;
using Logic.Shared.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Shared.Models.Authentication;

namespace Web.App.Bundels;

internal static class AuthRegistration
{
    public static void AddJwtAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("admin-access", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.IsInRole(UserRoleClaims.Admin) || ctx.User.IsInRole(UserRoleClaims.SystemAdmin)));

            options.AddPolicy("module:training:canview", policy =>
                policy.RequireAssertion(ctx => HasModuleScopeOrAdmin(ctx, "training:canview")));

            options.AddPolicy("module:health:canview", policy =>
                policy.RequireAssertion(ctx => HasModuleScopeOrAdmin(ctx, "health:canview")));

            options.AddPolicy("module:performance:canview", policy =>
                policy.RequireAssertion(ctx => HasModuleScopeOrAdmin(ctx, "performance:canview")));
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
