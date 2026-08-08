using Android.App;
using Android.App.Job;
using Android.Content;
using Web.Mobile.Services;

namespace Web.Mobile;

public sealed class AndroidBackgroundSyncScheduler : IBackgroundSyncScheduler
{
    private const int JobId = 45272;
    private readonly Context _context;

    public AndroidBackgroundSyncScheduler()
    {
        _context = Platform.AppContext;
    }

    public Task ScheduleAsync(int intervalMinutes, CancellationToken cancellationToken = default)
    {
        var scheduler = _context.GetSystemService(Context.JobSchedulerService) as JobScheduler;
        if (scheduler is null)
        {
            return Task.CompletedTask;
        }

        var intervalMs = (long)TimeSpan.FromMinutes(global::System.Math.Max(15, intervalMinutes)).TotalMilliseconds;
        var javaClass = Java.Lang.Class.FromType(typeof(BackgroundSyncJobService));
        if (javaClass is null)
        {
            return Task.CompletedTask;
        }

        var component = new ComponentName(_context, javaClass);
        var builder = new JobInfo.Builder(JobId, component);
        builder.SetPersisted(true);
        builder.SetRequiredNetworkType(NetworkType.Any);
        builder.SetPeriodic(intervalMs);
        var jobInfo = builder.Build();

        if (jobInfo is not null)
        {
            scheduler.Schedule(jobInfo);
        }
        return Task.CompletedTask;
    }

    public Task CancelAsync(CancellationToken cancellationToken = default)
    {
        var scheduler = _context.GetSystemService(Context.JobSchedulerService) as JobScheduler;
        scheduler?.Cancel(JobId);
        return Task.CompletedTask;
    }
}
