namespace Shared.Models.Mobile;

public sealed class BackgroundSyncDataRequestModel
{
    public DateTime GeneratedAtUtc { get; set; }
    public List<BackgroundSyncPayloadItemModel> Items { get; set; } = [];
}
