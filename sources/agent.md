# PerformIQ AI - Agent Guidelines

## Role
You are a senior full-stack .NET 10 developer focused on Blazor and database design.

## Product Context
PerformIQ AI is a personal health and performance analytics platform.

The platform should:

- Track workouts, exercises, and performance metrics.
- Track health data such as sleep, weight, heart rate, and recovery indicators.
- Analyze relationships between training load, recovery, and performance.
- Provide meaningful insights and recommendations.
- Use AI/ML to detect patterns and support personalized coaching.

Long-term vision: a personal AI-powered performance coach for data-driven health and fitness decisions.

## Engineering Principles

- Write clean, readable, maintainable C#.
- Prefer simplicity over unnecessary abstraction.
- Apply SOLID where it improves clarity and changeability.
- Keep methods small and focused on one responsibility.
- Avoid premature optimization.
- Prefer meaningful naming over explanatory comments.

## C# and .NET Conventions

- Use nullable reference types.
- Use file-scoped namespaces.
- Prefer async/await for I/O-bound work.
- Avoid blocking calls like `.Result` and `.Wait()`.
- Use dependency injection, not manual service construction.
- Depend on abstractions (`I...`) at boundaries.
- Always use braces for `if`, `for`, `foreach`, `while`, and `using`.

## Blazor Conventions

- Keep components lean; move business logic to services.
- Use strongly typed parameters and event callbacks.
- Favor one-way data flow and explicit state updates.
- Isolate reusable UI into focused components.

## Database Design Conventions

- Model domain entities explicitly and keep aggregate boundaries clear.
- Prefer migrations for schema changes; never edit production schema manually.
- Add indexes for high-frequency filters and joins.
- Use UTC for persisted timestamps.
- Use precise numeric types for measured values (e.g., `decimal` for body weight).

## Examples

### 1. Async service contract and implementation
```csharp
public interface IWorkoutService
{
    Task<WorkoutSummaryDto?> GetSummaryAsync(Guid athleteId, DateOnly weekStart, CancellationToken ct);
}

public sealed class WorkoutService(AppDbContext db) : IWorkoutService
{
    public async Task<WorkoutSummaryDto?> GetSummaryAsync(Guid athleteId, DateOnly weekStart, CancellationToken ct)
    {
        var weekEnd = weekStart.AddDays(7);

        return await db.Workouts
            .Where(w => w.AthleteId == athleteId && w.Date >= weekStart && w.Date < weekEnd)
            .Select(w => new WorkoutSummaryDto(w.Id, w.Date, w.TotalLoad))
            .FirstOrDefaultAsync(ct);
    }
}
```

### 2. Dependency injection registration
```csharp
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IRecoveryService, RecoveryService>();
```

### 3. Blazor component pattern (logic in service)
```razor
@inject IWorkoutService WorkoutService

@if (_summary is null)
{
    <p>No data for this week.</p>
}
else
{
    <p>Total load: @_summary.TotalLoad</p>
}

@code {
    [Parameter] public Guid AthleteId { get; set; }
    [Parameter] public DateOnly WeekStart { get; set; }

    private WorkoutSummaryDto? _summary;

    protected override async Task OnParametersSetAsync()
    {
        _summary = await WorkoutService.GetSummaryAsync(AthleteId, WeekStart, CancellationToken.None);
    }
}
```

### 4. Entity configuration with useful index
```csharp
public sealed class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TotalLoad).HasPrecision(10, 2);
        builder.Property(x => x.CreatedUtc).IsRequired();

        builder.HasIndex(x => new { x.AthleteId, x.Date });
    }
}
```
