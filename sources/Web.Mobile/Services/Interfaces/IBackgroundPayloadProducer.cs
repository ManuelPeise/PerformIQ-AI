using Shared.Models.Mobile;

namespace Web.Mobile.Services;

public interface IBackgroundPayloadProducer
{
    string Name { get; }
    Task<BackgroundSyncPayloadItemModel?> ProduceAsync(CancellationToken cancellationToken = default);
}
