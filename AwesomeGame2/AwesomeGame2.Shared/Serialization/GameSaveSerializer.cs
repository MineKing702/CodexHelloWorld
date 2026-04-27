using System;
using System.Text.Json;
using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Serialization
{
    public static class GameSaveSerializer
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false
        };

        public static string SaveToJson(GameSave save)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            return JsonSerializer.Serialize(save, JsonOptions);
        }

        public static GameSave LoadFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON is required.", nameof(json));
            }

            var loaded = JsonSerializer.Deserialize<GameSave>(json, JsonOptions);
            if (loaded == null)
            {
                throw new InvalidOperationException("Unable to deserialize GameSave JSON.");
            }

            return loaded;
        }
    }
}
