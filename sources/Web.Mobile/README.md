# Web.Mobile Documentation

`Web.Mobile` is the MAUI-based mobile client for PerformIQ Ai.

## Current app structure

### Pages (MVVM)
- `AuthenticationPage`
  - Login with username/email + password.
  - Login is disabled until a valid API base URL is configured.
- `SettingsPage`
  - Configure API base URL.
  - Select language (`EN`, `DE`), default `EN`.
  - HealthConnect SDK status is checked automatically when the page appears.
  - Request permissions.
  - Single **Save Settings** button persists API URL + language and is enabled only when values were modified.
- `LogPage`
  - HealthConnect SDK availability status.
  - Granted permissions.
  - Background task state snapshot (active, interval, last run, last status).

### Navigation behavior
- Unauthenticated: `Authentication` + `Settings` tabs only.
- Authenticated: `Log` tab is added.

### Theme
- Dark theme only (`AppTheme.Dark` is enforced in `App.xaml.cs`).

## Settings propagation behavior

Settings changes are propagated through shared singleton services and events.

Example:
1. API base URL missing → login is disabled.
2. User saves valid API base URL in `SettingsPage`.
3. `AuthenticationPage` receives `ApiBaseUrlChanged`.
4. Login command/button state is re-evaluated and enabled immediately.

## Localization

- Supported languages: `EN`, `DE`.
- Default language: `EN`.
- Localization is key-based (`AppTextKeys` + `LocalizationService`).
- Mobile auth endpoint returns `messageKey` so dialogs are localized client-side.

## Build Android APK

1. Build a release APK:
   ```powershell
   dotnet publish .\sources\Web.Mobile\Web.Mobile.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=apk
   ```
2. Find the APK in:
   - `sources\Web.Mobile\bin\Release\net10.0-android\publish\`
   - or `sources\Web.Mobile\bin\Release\net10.0-android\`

### Signed APK (distribution)

Use a keystore for a signed build:

```powershell
dotnet publish .\sources\Web.Mobile\Web.Mobile.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=apk -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=<keystore-path> -p:AndroidSigningStorePass=<store-password> -p:AndroidSigningKeyAlias=<key-alias> -p:AndroidSigningKeyPass=<key-password>
```

## Background task architecture (current + future definition)

Current Android scheduling uses `JobScheduler` with the existing background runner.

### Current flow
1. Mobile auth login stores refresh token securely.
2. Runner refreshes JWT when needed.
3. Runner loads `/api/mobile/background-sync/config`.
4. If active, payload producers collect data and sender posts to `/api/mobile/background-sync/data`.

### How to define future health-data background tasks

When adding new background tasks, follow this contract-first pattern:

1. **Define payload producer**
   - Implement `IBackgroundPayloadProducer`.
   - Set unique `Name`.
   - Return one `BackgroundSyncPayloadItemModel` per logical payload.

2. **Define payload schema**
   - Keep payload JSON normalized and versionable.
   - Include enough metadata for server-side processing (time range, source, schemaVersion if needed).

3. **Register producer**
   - Register in DI (`MauiProgram.cs`) as `IBackgroundPayloadProducer`.
   - Producer is automatically included by `BackgroundSyncRunner`.

4. **Define server handling**
   - Extend mobile data endpoint processing for new `PayloadType`.
   - Validate payload and log structured processing outcomes.

5. **Schedule control**
   - Keep scheduling controlled by remote config endpoint.
   - Respect Android minimum interval constraints (>=15 minutes).

6. **Error behavior**
   - Use explicit status updates in state store.
   - Do not silently swallow auth/config/send failures.

## Important limitation (current)

- Unsent payloads are not persisted yet (no durable queue).
