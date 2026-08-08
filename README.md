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

## Blazor authentication flow

- Login and registration UI routes:
  - `/auth/login`
  - `/auth/register`
- Refresh tokens are stored server-side and transported via **HttpOnly cookie**.
- The client keeps only the access token in memory and tries a silent refresh on startup.
- Protected routes redirect unauthenticated users to `/auth/login?returnUrl=...`.

## Prerequisites

1. .NET SDK 10
2. Docker Desktop (Compose stack with webapp + mysql + serilog)
3. Valid DB connection string and JWT settings in:
   - `sources/Web.App/Web.App/appsettings.json`
4. Configure default system admin seed data in `appsettings.Development.json` under `SystemAdminSeed`
   - `Password` is configured as cleartext and is hashed during startup before being stored

## Logging

- Serilog is configured as the host logger.
- Logs are emitted as structured JSON to console.
- In containers, logs are written to stdout/stderr and can be read with `docker logs`.

## UI theme assets

- Theme tokens and typography scale are centralized in:
  - `sources/Web.App/Web.App/wwwroot/css/app.css`
- Self-hosted font locations:
  - `sources/Web.App/Web.App/wwwroot/fonts/inter`
  - `sources/Web.App/Web.App/wwwroot/fonts/jetbrains-mono`

## Run locally

```bash
dotnet run --project .\sources\Web.App\Web.App\Web.App.csproj --launch-profile https
```

Swagger UI is available at:

- `https://localhost:7102/swagger`

## Run with Docker

```bash
docker compose up --build
```

PowerShell helpers:

```powershell
.\scripts\docker\up.ps1
.\scripts\docker\update.ps1
.\scripts\docker\clean.ps1
```

Batch helpers:

```bat
.\scripts\docker\start-docker-containers.bat
.\scripts\docker\update-docker-containers.bat
.\scripts\docker\delete-docker-containers.bat
```

App endpoint:

- `http://localhost:8080`
- `http://localhost:8081` (Serilog/Seq UI)
- From another device in your LAN: `http://<HOST_IP>:8080` (app), `<HOST_IP>:3306` (MySQL), `<HOST_IP>:8081` (Seq UI).

View logs:

```bash
docker compose logs -f webapp
```