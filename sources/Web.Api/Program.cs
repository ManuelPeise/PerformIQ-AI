using Web.Api.Bundels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ServiceRegistration.ConfigureServices(builder);

var app = builder.Build();

// Configure app.
await AppConfiguration.ConfigureApp(app);

app.Run();

