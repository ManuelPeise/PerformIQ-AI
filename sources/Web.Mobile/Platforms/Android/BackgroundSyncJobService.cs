using Android.App;
using Android.App.Job;
using Microsoft.Extensions.DependencyInjection;
using Web.Mobile.Services;

namespace Web.Mobile;

[Service(
    Name = "com.companyname.web.mobile.BackgroundSyncJobService",
    Permission = "android.permission.BIND_JOB_SERVICE",
    Exported = true)]
public sealed class BackgroundSyncJobService : JobService
{
    public override bool OnStartJob(JobParameters? @params)
    {
        _ = Task.Run(async () =>
        {
            var serviceProvider = MainApplication.CurrentServices;
            if (serviceProvider is null)
            {
                JobFinished(@params, true);
                return;
            }

            var runner = serviceProvider.GetService<IBackgroundSyncRunner>();
            if (runner is null)
            {
                JobFinished(@params, true);
                return;
            }

            var success = await runner.ExecuteAsync();
            JobFinished(@params, !success);
        });

        return true;
    }

    public override bool OnStopJob(JobParameters? @params)
    {
        return true;
    }
}
