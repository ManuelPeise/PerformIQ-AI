using Shared.Models.Mobile;

namespace Web.Mobile.Services;

public interface IBackgroundSyncSender
{
    Task<bool> SendAsync(BackgroundSyncDataRequestModel requestModel, CancellationToken cancellationToken = default);
}
