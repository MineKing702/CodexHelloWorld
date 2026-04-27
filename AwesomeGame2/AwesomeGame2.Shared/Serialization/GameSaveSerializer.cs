using System.Text.Json;
using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Serialization;

public static class GameSaveSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false
    };

    public static string SaveToJson(GameSave save)
    {
        ArgumentNullException.ThrowIfNull(save);
        return JsonSerializer.Serialize(save, JsonOptions);
    }

    public static GameSave LoadFromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var loaded = JsonSerializer.Deserialize<GameSave>(json, JsonOptions);
        return loaded ?? throw new InvalidOperationException("Unable to deserialize GameSave JSON.");
    }
}
