using Shared.Models.Mobile;

namespace Web.Mobile.Services;

public interface IBackgroundSyncConfigClient
{
    Task<BackgroundSyncConfigResponseModel?> GetConfigAsync(CancellationToken cancellationToken = default);
}
