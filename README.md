# PerformIQ-AI

PerformIQ-AI is an ASP.NET Core 10 application with a Blazor front end, JWT authentication, and modular business logic for user/profile management.

## Project structure

- `sources/Web.App/Web.App` - main ASP.NET Core host (API endpoints, auth, Swagger, startup wiring)
- `sources/Web.App/Web.App.Client` - Blazor WebAssembly client
- `sources/Logic.Authentication` - authentication logic (register/login/refresh/revoke)
- `sources/Logic.Modules` - business modules (Profile, User Management)
- `sources/Data.Accessor` - repository and unit-of-work abstraction
- `sources/Data.Database` - EF Core entities, context, and seeding
- `sources/Shared.Models` - request/response models shared across layers

## API modules

- **Authentication**: `/api/auth/*`
- **Profile**: `/api/profile`
- **User Management** (admin): `/api/users*`

## Prerequisites

1. .NET SDK 10
2. MySQL running locally
3. Valid DB connection string and JWT settings in:
   - `sources/Web.App/Web.App/appsettings.json`

## Run locally

```bash
dotnet run --project .\sources\Web.App\Web.App\Web.App.csproj --launch-profile https
```

Swagger UI is available at:

- `https://localhost:7102/swagger`