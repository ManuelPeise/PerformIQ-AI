namespace Web.Mobile.Services;

public sealed class BackgroundSyncStateSnapshot
{
    public bool IsActive { get; init; }
    public int IntervalMinutes { get; init; }
    public DateTime? LastRunAtUtc { get; init; }
    public string LastStatus { get; init; } = "idle";
}
