namespace Web.Mobile.Services;

public sealed class BackgroundSyncStateStore : IBackgroundSyncStateStore
{
    private const string IsActiveKey = "bg-sync.is-active";
    private const string IntervalKey = "bg-sync.interval";
    private const string LastRunKey = "bg-sync.last-run";
    private const string LastStatusKey = "bg-sync.last-status";

    public BackgroundSyncStateSnapshot GetSnapshot()
    {
        var isActive = Preferences.Default.Get(IsActiveKey, false);
        var interval = Preferences.Default.Get(IntervalKey, 15);
        var lastRunRaw = Preferences.Default.Get(LastRunKey, string.Empty);
        var status = Preferences.Default.Get(LastStatusKey, "idle");

        DateTime? lastRun = null;
        if (DateTime.TryParse(lastRunRaw, out var parsed))
        {
            lastRun = DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
        }

        return new BackgroundSyncStateSnapshot
        {
            IsActive = isActive,
            IntervalMinutes = interval,
            LastRunAtUtc = lastRun,
            LastStatus = status
        };
    }

    public void SetScheduleState(bool isActive, int intervalMinutes)
    {
        Preferences.Default.Set(IsActiveKey, isActive);
        Preferences.Default.Set(IntervalKey, Math.Max(15, intervalMinutes));
    }

    public void SetLastRun(string status, DateTime? runAtUtc = null)
    {
        var utcValue = runAtUtc ?? DateTime.UtcNow;
        Preferences.Default.Set(LastStatusKey, status);
        Preferences.Default.Set(LastRunKey, utcValue.ToString("O"));
    }
}
