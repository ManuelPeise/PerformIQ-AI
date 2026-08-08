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
- `sources/Web.Mobile` - .NET MAUI mobile app (Android/iOS/Windows targets)
- `sources/Shared.AndroidBindings` - .NET Android binding project for Health Connect bridge
- `sources/Android.HealthConnectApp/sources` - native Android Gradle project producing the bridge AAR

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
2. Docker Desktop (Compose stack with webapp + mysql + seqlog)
3. Valid DB connection string and JWT settings in:
   - `sources/Web.App/Web.App/appsettings.json`
4. Configure default system admin seed data in `appsettings.Development.json` under `SystemAdminSeed`
   - `Password` is configured as cleartext and is hashed during startup before being stored
5. For Android bridge/mobile work: Android SDK + Java 17+ (Android Studio recommended)

## Logging

- Seq.Extensions.Logging is configured as a host logging provider.
- Seq target configuration is read from the `Seq` section in appsettings/environment.
- In containers, logs are shipped to the `seqlog` service and can be explored in Seq UI.

## UI theme assets

- Theme tokens and typography scale are centralized in:
  - `sources/Web.App/Web.App/wwwroot/css/app.css`
- Self-hosted font locations:
  - `sources/Web.App/Web.App/wwwroot/fonts/inter`
  - `sources/Web.App/Web.App/wwwroot/fonts/jetbrains-mono`

## Android Health Connect bridge workflow

- Native Android Gradle project:
  - `sources/Android.HealthConnectApp/sources`
  - modules: `:healthconnectbridge` (library) and `:app` (sample app)
- Build/copy `.aar`:
  1. `cd sources/Android.HealthConnectApp/sources`
  2. `./gradlew :healthconnectbridge:copyReleaseAarToBindings` (or `gradlew.bat` on Windows)
- AAR destination for .NET binding:
  - `sources/Shared.AndroidBindings/Jars/healthconnectbridge-release.aar`
- MAUI consumption:
  - `sources/Web.Mobile` uses `Shared.AndroidBindings` on Android target and resolves calls through `IHealthConnectBridgeService`.
- Android permission rationale handlers are registered in:
  - `sources/Web.Mobile/Platforms/Android/AndroidManifest.xml`
  - `sources/Web.Mobile/Platforms/Android/PermissionsRationaleActivity.cs`
- Mobile app branding:
  - Application title: `PerformIq Ai`
  - App icon source: `sources/Web.Mobile/Resources/AppIcon/appicon.png`

## Mobile background sync (Android)

- API endpoints:
  - `POST /api/mobile/auth/login` (returns access + refresh token in response body)
  - `POST /api/mobile/auth/refresh` (uses refresh token from request body)
  - `GET /api/mobile/background-sync/config` (currently dummy config endpoint)
  - `POST /api/mobile/background-sync/data` (receives generic background payload)
- Mobile flow:
  1. User logs in from the mobile Auth UI.
  2. Refresh token is stored in secure storage.
  3. Background job refreshes JWT when needed.
  4. Job loads sync config, schedules/updates periodic execution, and sends produced payload items.
- Android scheduling:
  - Implemented via `JobScheduler` (`BackgroundSyncJobService` + `AndroidBackgroundSyncScheduler`).
  - Minimum interval is clamped to 15 minutes.
- Current limitation:
  - Unsent payloads are not persisted yet.

## Mobile UI (MVVM pages)

- `Web.Mobile` now uses tab-based navigation with MVVM pages:
  - `Authentication` (login)
  - `Settings` (API base URL + HealthConnect status/permissions)
  - `Log` (SDK status icon, granted permissions, background-task snapshot)
- Unauthenticated state:
  - `Authentication` and `Settings` tabs are visible.
  - `Log` tab is hidden until authentication succeeds.
- Startup splash page shows:
  - app icon
  - app name (`PerformIq Ai`)

Detailed mobile documentation:
- `sources/Web.Mobile/README.md`

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
- `http://localhost:8081` (Seq UI)
- From another device in your LAN: `http://<HOST_IP>:8080` (app), `<HOST_IP>:3306` (MySQL), `<HOST_IP>:8081` (Seq UI).

View logs:

```bash
docker compose logs -f webapp
```