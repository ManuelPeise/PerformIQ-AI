using Web.Api.Services.ApiControllers.Authentication;

namespace Web.Api.Bundels
{
    internal static class AppConfiguration
    {
        private const string CorsPolicyName = "AllowAll";
        internal static async Task ConfigureApp(WebApplication app)
        {
            await app.InitializeDatabaseAsync();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PerformIQ API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseCors(CorsPolicyName);

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            AuthenticationEndpointMapper.MapAuthEndpoints(app);

            app.MapGet("/", () => "PerformIQ API is running");
        }
    }
}
