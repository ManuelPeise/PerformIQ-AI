namespace Web.Mobile.Services;

public sealed class BackgroundSyncScheduler : IBackgroundSyncScheduler
{
    public Task ScheduleAsync(int intervalMinutes, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task CancelAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
