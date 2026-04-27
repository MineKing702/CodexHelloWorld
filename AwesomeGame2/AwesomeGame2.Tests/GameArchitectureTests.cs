using System.Reflection;
using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Content;
using AwesomeGame2.Shared.Managers;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Serialization;
using AwesomeGame2.Shared.Validation;

namespace AwesomeGame2.Tests;

public static class GameArchitectureTests
{
    public static void RunAll()
    {
        NewGameCreation_ProducesValidDefaultSave();
        SaveLoadJson_RoundTripsGameSave();
        FileSaveLoad_UsesTemporaryFiles();
        SaveMigration_HandlesLegacyVersion();
        InvalidSaveValidation_RepairsAndRejectsDeliberately();
        MutableGameplayState_IsRootedInGameSave();
        EveryScreen_IsGeneratedFromGameSave();
        ActiveBattle_SurvivesSaveLoad();
        DailyActionReset_OnNewDay();
        LevelUp_Behavior();
        InvalidCommands_ReturnSafeErrorsWithoutCorruption();
        StaticContent_IsNotPlayerMutableState();
    }

    private static void NewGameCreation_ProducesValidDefaultSave()
    {
        var save = GameSaveFactory.CreateNew("Hero");

        Assert(save.SaveVersion == GameSave.CurrentSaveVersion, "Save version mismatch.");
        Assert(save.CurrentLocation == "main_menu", "Starting location mismatch.");
        Assert(save.RemainingDailyActions == GameRules.DailyActionLimit, "Daily action limit mismatch.");
        Assert(GameSaveValidator.IsValid(save), "New save should be valid.");
    }

    private static void SaveLoadJson_RoundTripsGameSave()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        var manager = new GameManager();
        manager.Execute(save, new MenuCommand(CommandIds.NewGame));
        manager.Execute(save, new MenuCommand(CommandIds.Adventure));

        var json = GameSaveSerializer.SaveToJson(save);
        var roundTrip = GameSaveSerializer.LoadFromJson(json);

        Assert(roundTrip.Player.Name == save.Player.Name, "Round-trip player name mismatch.");
        Assert(roundTrip.CurrentLocation == save.CurrentLocation, "Round-trip location mismatch.");
        Assert(roundTrip.ActiveBattleState is not null, "Round-trip battle state missing.");
    }

    private static void FileSaveLoad_UsesTemporaryFiles()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        var path = Path.Combine(Path.GetTempPath(), $"awesomegame2-{Guid.NewGuid():N}.json");

        try
        {
            GameSaveSerializer.SaveToFile(save, path);
            var loaded = GameSaveSerializer.LoadFromFile(path);
            Assert(loaded.Player.Name == "Hero", "File load mismatch.");
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static void SaveMigration_HandlesLegacyVersion()
    {
        var json = """
                   {
                     "saveVersion": 0,
                     "player": { "name": "Legacy", "level": 1, "experience": 0, "gold": 0, "health": 30, "maxHealth": 30 },
                     "currentLocation": "town",
                     "dayNumber": 1,
                     "remainingDailyActions": 50,
                     "inventory": [],
                     "equipment": { "weaponId": "rusty_sword", "armorId": "cloth_tunic" },
                     "activeBattleState": null,
                     "progressionFlags": { "flags": [] },
                     "randomState": { "seed": 1, "calls": 0 }
                   }
                   """;

        var migrated = GameSaveSerializer.LoadFromJson(json);
        Assert(migrated.SaveVersion == GameSave.CurrentSaveVersion, "Legacy migration should set current version.");
        Assert(migrated.RemainingDailyActions == GameRules.DailyActionLimit, "Legacy migration should clamp actions.");
    }

    private static void InvalidSaveValidation_RepairsAndRejectsDeliberately()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        save.RemainingDailyActions = -12;
        save.Equipment.WeaponId = "bad_weapon";

        GameSaveValidator.Repair(save);
        Assert(save.RemainingDailyActions == 0, "Repair should clamp actions to zero minimum.");
        Assert(save.Equipment.WeaponId == "rusty_sword", "Repair should reset unknown weapon.");

        var tooNew = GameSaveFactory.CreateNew("Hero");
        tooNew.SaveVersion = GameSave.CurrentSaveVersion + 1;
        var json = GameSaveSerializer.SaveToJson(tooNew);

        AssertThrows(() => GameSaveSerializer.LoadFromJson(json), "Too-new saves should be rejected.");
    }

    private static void MutableGameplayState_IsRootedInGameSave()
    {
        var gameSaveProps = typeof(GameSave).GetProperties().Select(p => p.PropertyType).ToArray();

        Assert(gameSaveProps.Contains(typeof(AwesomeGame2.Shared.Models.PlayerData)), "PlayerData must be rooted in GameSave.");
        Assert(gameSaveProps.Contains(typeof(AwesomeGame2.Shared.Models.EquipmentState)), "EquipmentState must be rooted in GameSave.");
        Assert(gameSaveProps.Contains(typeof(AwesomeGame2.Shared.Models.ProgressionFlags)), "ProgressionFlags must be rooted in GameSave.");
        Assert(gameSaveProps.Contains(typeof(AwesomeGame2.Shared.Models.RandomState)), "RandomState must be rooted in GameSave.");

        var managerFields = typeof(GameManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert(managerFields.All(f => f.IsInitOnly), "Managers must not keep mutable gameplay fields.");
    }

    private static void EveryScreen_IsGeneratedFromGameSave()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        var screenManager = new ScreenManager();

        save.CurrentLocation = "main_menu";
        Assert(screenManager.CreateForCurrentLocation(save).ScreenId == "main_menu", "Main menu screen missing.");

        save.CurrentLocation = "town";
        Assert(screenManager.CreateForCurrentLocation(save).ScreenId == "town", "Town screen missing.");

        save.CurrentLocation = "character";
        Assert(screenManager.CreateForCurrentLocation(save).ScreenId == "character", "Character screen missing.");

        save.ActiveBattleState = new AwesomeGame2.Shared.Models.BattleState
        {
            EnemyId = GameContent.Enemies[0].Id,
            EnemyName = GameContent.Enemies[0].Name,
            EnemyHealth = 5,
            EnemyMaxHealth = 5,
            IsPlayerTurn = true
        };
        save.CurrentLocation = "battle";
        Assert(screenManager.CreateForCurrentLocation(save).ScreenId == "battle", "Battle screen missing.");

        save.CurrentLocation = "level_up";
        Assert(screenManager.CreateForCurrentLocation(save).ScreenId == "level_up", "Level-up screen missing.");

        save.CurrentLocation = "death";
        Assert(screenManager.CreateForCurrentLocation(save).ScreenId == "death", "Death screen missing.");

        save.CurrentLocation = "error";
        Assert(screenManager.CreateForCurrentLocation(save).ScreenId == "error", "Error screen missing.");

        var saveLoad = screenManager.CreateSaveLoadResultScreen(save, true, "Save", "temp.json");
        Assert(saveLoad.ScreenId == "save_load_result", "Save/load result screen missing.");
    }

    private static void ActiveBattle_SurvivesSaveLoad()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        var manager = new GameManager();

        manager.Execute(save, new MenuCommand(CommandIds.NewGame));
        manager.Execute(save, new MenuCommand(CommandIds.Adventure));
        var path = Path.Combine(Path.GetTempPath(), $"awesomegame2-battle-{Guid.NewGuid():N}.json");

        try
        {
            GameSaveSerializer.SaveToFile(save, path);
            var loaded = GameSaveSerializer.LoadFromFile(path);
            Assert(loaded.ActiveBattleState is not null, "Active battle must survive save/load.");
            Assert(loaded.CurrentLocation == "battle", "Battle location must survive save/load.");
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static void DailyActionReset_OnNewDay()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        var manager = new GameManager();

        manager.Execute(save, new MenuCommand(CommandIds.NewGame));
        manager.Execute(save, new MenuCommand(CommandIds.Adventure));
        Assert(save.RemainingDailyActions == GameRules.DailyActionLimit - 1, "Adventure should consume one action.");

        manager.Execute(save, new MenuCommand(CommandIds.Rest));
        Assert(save.RemainingDailyActions == GameRules.DailyActionLimit, "Rest should reset daily actions.");
        Assert(save.DayNumber == 2, "Rest should advance day.");
    }

    private static void LevelUp_Behavior()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        var manager = new GameManager();

        save.Player.Experience = GameRules.XpPerLevel - 1;
        save.ActiveBattleState = new AwesomeGame2.Shared.Models.BattleState
        {
            EnemyId = GameContent.Enemies[0].Id,
            EnemyName = GameContent.Enemies[0].Name,
            EnemyHealth = 1,
            EnemyMaxHealth = 10,
            IsPlayerTurn = true
        };
        save.CurrentLocation = "battle";

        manager.Execute(save, new MenuCommand(CommandIds.Attack));
        Assert(save.Player.Level >= 2, "Player should level up after enough xp gain.");
        Assert(save.CurrentLocation == "level_up", "Level-up should route to level-up screen.");
    }

    private static void InvalidCommands_ReturnSafeErrorsWithoutCorruption()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        var manager = new GameManager();
        var baseline = GameSaveSerializer.SaveToJson(save);

        var result = manager.Execute(save, new MenuCommand("definitely_invalid"));
        var after = GameSaveSerializer.SaveToJson(save);

        Assert(result.Error is not null, "Invalid command should include error.");
        Assert(save.CurrentLocation == "error", "Invalid command should route to error screen.");

        var baselineState = GameSaveSerializer.LoadFromJson(baseline);
        Assert(baselineState.Player.Name == save.Player.Name, "Player state should remain unchanged on invalid command.");
        Assert(after.Contains("\"currentLocation\": \"error\"", StringComparison.Ordinal), "Only safe screen location should change.");
    }

    private static void StaticContent_IsNotPlayerMutableState()
    {
        Assert(GameContent.Enemies.Count >= 3, "Expected at least three enemies.");
        Assert(GameContent.Weapons.Count >= 3, "Expected at least three weapons.");
        Assert(GameContent.Armor.Count >= 3, "Expected at least three armor definitions.");

        var weaponSetters = typeof(WeaponDefinition).GetProperties().Where(p => p.SetMethod is not null).ToList();
        Assert(weaponSetters.Count == 0, "Weapon definitions should be immutable.");
    }

    private static void AssertThrows(Action action, string message)
    {
        try
        {
            action();
        }
        catch
        {
            return;
        }

        throw new InvalidOperationException(message);
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
