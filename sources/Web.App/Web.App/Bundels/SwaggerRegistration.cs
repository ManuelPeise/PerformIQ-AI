using Microsoft.OpenApi.Models;

namespace Web.App.Bundels;

internal static class SwaggerRegistration
{
    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PerformIQ API",
                Version = "1.0.0"
            });
        });
    }
}
