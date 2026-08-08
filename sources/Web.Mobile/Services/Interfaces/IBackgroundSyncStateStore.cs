namespace Web.Mobile.Services;

public interface IBackgroundSyncStateStore
{
    BackgroundSyncStateSnapshot GetSnapshot();
    void SetScheduleState(bool isActive, int intervalMinutes);
    void SetLastRun(string status, DateTime? runAtUtc = null);
}
