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

- JWT-based authentication and refresh-token flow
- Role-based admin policy (`admin-access`) for user-management endpoints
- Module-scope policies for feature permissions

## Logging

- Serilog is used as the host logging provider.
- Logs are emitted as JSON to the console.
- Docker usage is aligned with stdout/stderr log collection (`docker logs` / `docker compose logs`).

## Local development

1. Ensure Docker is available (single container runs WebApp + MariaDB).
2. Configure JWT settings in `sources/Web.App/Web.App/appsettings.json`.
3. Configure `SystemAdminSeed` in `sources/Web.App/Web.App/appsettings.Development.json` for local development.
   - `SystemAdminSeed.Password` is configured as cleartext and gets hashed on startup before persistence.
4. Start containerized stack:

```bash
docker compose up --build
```

5. Start the app:
   - The app starts inside Docker as part of step 4.

6. Open Swagger:

- `http://localhost:8080/swagger`

## Docker

Run:

```bash
docker compose up --build
```

App endpoint:

- `http://localhost:8080`
- LAN access: `http://<HOST_IP>:8080` (and `<HOST_IP>:3306` for DB if firewall allows).

Tail logs:

```bash
docker compose logs -f app
```

PowerShell helper scripts:

```powershell
.\scripts\docker\up.ps1
.\scripts\docker\update.ps1
.\scripts\docker\clean.ps1
```

Batch helper scripts:

```bat
.\scripts\docker\start-docker-containers.bat
.\scripts\docker\update-docker-containers.bat
.\scripts\docker\delete-docker-containers.bat
```

## Conventions

- Place API endpoint registrations in `sources/Web.App/Web.App/ApiControllers`.
- Prefer `IApplicationUnitOfWork` for database operations in business logic (where applicable).
