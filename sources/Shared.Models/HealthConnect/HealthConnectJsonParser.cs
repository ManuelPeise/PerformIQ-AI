using System.Text.Json;

namespace Shared.Models.HealthConnect;

public static class HealthConnectJsonParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static HealthConnectReadResponseModel Parse(string json)
    {
        var model = JsonSerializer.Deserialize<HealthConnectReadResponseModel>(json, JsonOptions);
        if (model is null)
        {
            throw new InvalidOperationException("Unable to parse HealthConnect payload.");
        }

        return model;
    }

    public static bool TryParse(
        string json,
        out HealthConnectReadResponseModel? model,
        out string? errorMessage)
    {
        try
        {
            model = Parse(json);
            errorMessage = null;
            return true;
        }
        catch (Exception ex)
        {
            model = null;
            errorMessage = ex.Message;
            return false;
        }
    }
}
