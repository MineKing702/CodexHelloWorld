using AwesomeGame2.Shared.Content;

namespace AwesomeGame2.Shared.Saves;

public static class GameSaveMigrator
{
    public static GameSave Migrate(GameSave save)
    {
        ArgumentNullException.ThrowIfNull(save);

        if (save.SaveVersion > GameSave.CurrentSaveVersion)
        {
            throw new InvalidOperationException($"Unsupported save version {save.SaveVersion}. Current version is {GameSave.CurrentSaveVersion}.");
        }

        if (save.SaveVersion <= 0)
        {
            save.SaveVersion = 1;
        }

        save.SaveVersion = GameSave.CurrentSaveVersion;

        // Migration hook for future versions.
        if (save.RemainingDailyActions > GameRules.DailyActionLimit)
        {
            save.RemainingDailyActions = GameRules.DailyActionLimit;
        }

        return save;
    }
}
