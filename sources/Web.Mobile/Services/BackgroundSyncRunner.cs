using Shared.Models.Mobile;

namespace Web.Mobile.Services;

public sealed class BackgroundSyncRunner : IBackgroundSyncRunner
{
    private readonly IBackgroundSyncConfigClient _configClient;
    private readonly IBackgroundSyncSender _sender;
    private readonly IEnumerable<IBackgroundPayloadProducer> _payloadProducers;
    private readonly IBackgroundSyncScheduler _scheduler;
    private readonly IBackgroundSyncStateStore _stateStore;

    public BackgroundSyncRunner(
        IBackgroundSyncConfigClient configClient,
        IBackgroundSyncSender sender,
        IEnumerable<IBackgroundPayloadProducer> payloadProducers,
        IBackgroundSyncScheduler scheduler,
        IBackgroundSyncStateStore stateStore)
    {
        _configClient = configClient;
        _sender = sender;
        _payloadProducers = payloadProducers;
        _scheduler = scheduler;
        _stateStore = stateStore;
    }

    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var config = await _configClient.GetConfigAsync(cancellationToken);
            if (config is null)
            {
                _stateStore.SetLastRun("config-unavailable");
                return false;
            }

            var intervalMinutes = Math.Max(15, config.IntervalMinutes);
            _stateStore.SetScheduleState(config.IsActive, intervalMinutes);

            if (!config.IsActive)
            {
                await _scheduler.CancelAsync(cancellationToken);
                _stateStore.SetLastRun("inactive");
                return true;
            }

            await _scheduler.ScheduleAsync(intervalMinutes, cancellationToken);

            var requestModel = new BackgroundSyncDataRequestModel
            {
                GeneratedAtUtc = DateTime.UtcNow
            };

            foreach (var producer in _payloadProducers)
            {
                var item = await producer.ProduceAsync(cancellationToken);
                if (item is not null)
                {
                    requestModel.Items.Add(item);
                }
            }

            if (requestModel.Items.Count == 0)
            {
                _stateStore.SetLastRun("no-items");
                return true;
            }

            var sent = await _sender.SendAsync(requestModel, cancellationToken);
            _stateStore.SetLastRun(sent ? "sent" : "send-failed");
            return sent;
        }
        catch
        {
            _stateStore.SetLastRun("error");
            return false;
        }
    }
}
