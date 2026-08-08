using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Web.App.Client.Auth;
using Web.App.Client.Pages.Auth.ViewModels;
using Web.App.Client.Pages.ViewModels;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ClientAuthSessionService>();
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthenticationStateProvider>();
builder.Services.AddScoped<IClientAuthenticationService, ClientAuthenticationService>();
builder.Services.AddScoped<AuthHttpClient>();
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<RegisterViewModel>();
builder.Services.AddScoped<HomeViewModel>();

var host = builder.Build();
var authenticationService = host.Services.GetRequiredService<IClientAuthenticationService>();
await authenticationService.InitializeAsync();
await host.RunAsync();
