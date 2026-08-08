namespace Web.Mobile.Services;

public interface IBackgroundSyncRunner
{
    Task<bool> ExecuteAsync(CancellationToken cancellationToken = default);
}
