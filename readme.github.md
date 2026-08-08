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
- `sources/Web.Mobile` - .NET MAUI mobile app
- `sources/Shared.AndroidBindings` - Android binding project for Health Connect bridge
- `sources/Android.HealthConnectApp/sources` - native Android Gradle project for bridge AAR generation

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

- Seq.Extensions.Logging is used as a host logging provider.
- Logs are emitted as JSON to the console.
- Docker usage is aligned with stdout/stderr log collection (`docker logs` / `docker compose logs`).

## Local development

1. Ensure Docker is available (Compose stack runs `webapp`, `mysql`, and `seqlog`).
2. Configure JWT settings in `sources/Web.App/Web.App/appsettings.json`.
3. Configure `SystemAdminSeed` in `sources/Web.App/Web.App/appsettings.Development.json` for local development.
   - `SystemAdminSeed.Password` is configured as cleartext and gets hashed on startup before persistence.
4. For Android bridge/mobile work, install Android SDK and Java 17+ (Android Studio recommended).
5. Start containerized stack:

```bash
docker compose up --build
```

6. Start the app:
   - The app starts inside Docker as part of step 4 (`webapp` service).

7. Open Swagger:

- `http://localhost:8080/swagger`

## Docker

Run:

```bash
docker compose up --build
```

App endpoint:

- `http://localhost:8080`
- `http://localhost:8081` (Seq UI)
- LAN access: `http://<HOST_IP>:8080` (app), `<HOST_IP>:3306` (MySQL), `<HOST_IP>:8081` (Seq UI), if firewall allows.

Tail logs:

```bash
docker compose logs -f webapp
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

## Mobile / Health Connect notes

- Build and copy Android bridge AAR:
  - `cd sources/Android.HealthConnectApp/sources`
  - `gradlew.bat :healthconnectbridge:copyReleaseAarToBindings` (Windows) / `./gradlew ...` (Unix)
- AAR is consumed from `sources/Shared.AndroidBindings/Jars/healthconnectbridge-release.aar`.
- Health Connect permission rationale handlers are defined in:
  - `sources/Web.Mobile/Platforms/Android/AndroidManifest.xml`
  - `sources/Web.Mobile/Platforms/Android/PermissionsRationaleActivity.cs`
- Current MAUI branding:
  - App title: `PerformIq Ai`
  - App icon source: `sources/Web.Mobile/Resources/AppIcon/appicon.png`

## Mobile background sync (Android)

- Auth + config/data endpoints:
  - `POST /api/mobile/auth/login`
  - `POST /api/mobile/auth/refresh`
  - `GET /api/mobile/background-sync/config` (dummy config response for now)
  - `POST /api/mobile/background-sync/data`
- Mobile implementation details:
  - Refresh token is stored with secure storage on device.
  - Access token is renewed by refresh flow before background API calls.
  - Generic payload producer pipeline is used before sending data.
  - Android scheduler uses `JobScheduler` to run while app is closed.
- Current limitation:
  - Unsent payloads are currently not persisted.

## Mobile pages (MVVM)

- `Web.Mobile` uses MVVM pages with tab navigation:
  - Authentication
  - Settings
  - Log
- While unauthenticated, only Authentication + Settings are shown; Log is hidden.
- Splash startup experience shows app icon and app name (`PerformIq Ai`).
- Detailed mobile documentation: `sources/Web.Mobile/README.md`.
