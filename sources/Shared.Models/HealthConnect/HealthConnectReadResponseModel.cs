using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Models.HealthConnect;

public sealed class HealthConnectReadResponseModel
{
    [JsonPropertyName("schemaVersion")]
    public string SchemaVersion { get; set; } = string.Empty;

    [JsonPropertyName("generatedAtUtc")]
    public DateTimeOffset? GeneratedAtUtc { get; set; }

    [JsonPropertyName("timeRange")]
    public HealthConnectTimeRangeModel? TimeRange { get; set; }

    [JsonPropertyName("recordCount")]
    public int RecordCount { get; set; }

    [JsonPropertyName("records")]
    public List<HealthConnectRecordModel> Records { get; set; } = [];

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

public sealed class HealthConnectTimeRangeModel
{
    [JsonPropertyName("startTimeUtc")]
    public DateTimeOffset? StartTimeUtc { get; set; }

    [JsonPropertyName("endTimeUtc")]
    public DateTimeOffset? EndTimeUtc { get; set; }
}

public sealed class HealthConnectRecordModel
{
    [JsonPropertyName("recordType")]
    public string RecordType { get; set; } = string.Empty;

    [JsonPropertyName("startTimeUtc")]
    public DateTimeOffset? StartTimeUtc { get; set; }

    [JsonPropertyName("endTimeUtc")]
    public DateTimeOffset? EndTimeUtc { get; set; }

    [JsonPropertyName("startZoneOffset")]
    public string? StartZoneOffset { get; set; }

    [JsonPropertyName("endZoneOffset")]
    public string? EndZoneOffset { get; set; }

    [JsonPropertyName("timeUtc")]
    public DateTimeOffset? TimeUtc { get; set; }

    [JsonPropertyName("zoneOffset")]
    public string? ZoneOffset { get; set; }

    [JsonPropertyName("sourceApp")]
    public string SourceApp { get; set; } = string.Empty;

    [JsonPropertyName("lastModifiedTimeUtc")]
    public DateTimeOffset? LastModifiedTimeUtc { get; set; }

    [JsonPropertyName("values")]
    public Dictionary<string, JsonElement> Values { get; set; } = [];
}
