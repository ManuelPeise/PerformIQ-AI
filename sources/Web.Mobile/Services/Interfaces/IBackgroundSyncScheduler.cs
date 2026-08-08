namespace Web.Mobile.Services;

public interface IBackgroundSyncScheduler
{
    Task ScheduleAsync(int intervalMinutes, CancellationToken cancellationToken = default);
    Task CancelAsync(CancellationToken cancellationToken = default);
}
