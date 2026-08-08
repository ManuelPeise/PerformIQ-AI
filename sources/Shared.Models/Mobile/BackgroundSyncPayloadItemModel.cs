using System.Text.Json;

namespace Shared.Models.Mobile;

public sealed class BackgroundSyncPayloadItemModel
{
    public string Producer { get; set; } = string.Empty;
    public string PayloadType { get; set; } = string.Empty;
    public DateTime CollectedAtUtc { get; set; }
    public JsonElement Payload { get; set; }
}
