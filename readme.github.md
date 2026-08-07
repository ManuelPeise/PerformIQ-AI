# PerformIQ-AI (GitHub Documentation)

PerformIQ-AI is a modular ASP.NET Core 10 + Blazor application focused on authentication, profile management, and user administration.

## Repository layout

- `sources/Web.App/Web.App` - ASP.NET Core host, API endpoint registration, auth, Swagger
- `sources/Web.App/Web.App.Client` - Blazor WebAssembly client
- `sources/Logic.Authentication` - JWT auth and refresh-token flows
- `sources/Logic.Modules` - business modules (`ProfileService`, `UserManagement`)
- `sources/Data.Accessor` - repository + unit-of-work abstraction
- `sources/Data.Database` - EF Core entities, context, seeding
- `sources/Shared.Models` - shared DTOs/contracts

## Current API surface

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/revoke`
- `GET /api/profile`
- `PUT /api/profile`
- `GET /api/users`
- `GET /api/users/{userId}`
- `PUT /api/users/{userId}/active`
- `PUT /api/users/{userId}/roles`

## Security and authorization

- JWT Bearer authentication
- Role-based admin policy (`admin-access`) for user-management endpoints
- Module-scope policies for feature permissions

## Local development

1. Configure MySQL connection string and JWT settings in `sources/Web.App/Web.App/appsettings.json`.
2. Start the app:

```bash
dotnet run --project .\sources\Web.App\Web.App\Web.App.csproj --launch-profile https
```

3. Open Swagger:

- `https://localhost:7102/swagger`

## Conventions

- Place API endpoint registrations in `sources/Web.App/Web.App/ApiControllers`.
- Prefer `IApplicationUnitOfWork` for database operations in business logic (where applicable).
