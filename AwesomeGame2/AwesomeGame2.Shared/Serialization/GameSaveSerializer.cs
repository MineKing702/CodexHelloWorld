using System.Text.Json;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Validation;

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

        var loaded = JsonSerializer.Deserialize<GameSave>(json, JsonOptions)
            ?? throw new InvalidOperationException("Unable to deserialize GameSave JSON.");

        var migrated = GameSaveMigrator.Migrate(loaded);
        GameSaveValidator.Repair(migrated);
        var errors = GameSaveValidator.Validate(migrated);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException($"Invalid GameSave after load: {string.Join("; ", errors)}");
        }

        return migrated;
    }

    public static void SaveToFile(GameSave save, string path)
    {
        ArgumentNullException.ThrowIfNull(save);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        File.WriteAllText(path, SaveToJson(save));
    }

    public static GameSave LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var json = File.ReadAllText(path);
        return LoadFromJson(json);
    }
}
