using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Managers;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Serialization;
using AwesomeGame2.Shared.Validation;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Tests;

public static class GameArchitectureTests
{
    public static void RunAll()
    {
        NewGameCreation_ProducesValidDefaultSave();
        GameSaveValidation_ReturnsErrorsForInvalidState();
        SaveLoadJson_RoundTripsGameSave();
        ViewModels_AreGeneratedFromGameSave();
        Navigation_MutatesCurrentLocationInGameSave();
        Managers_DoNotRequireMutableConstructorState();
    }

    private static void NewGameCreation_ProducesValidDefaultSave()
    {
        var save = GameSaveFactory.CreateNew("Hero");

        Assert(save.SaveVersion == GameSave.CurrentSaveVersion, "Save version mismatch.");
        Assert(save.Player.Name == "Hero", "Player name mismatch.");
        Assert(save.CurrentLocation == "town", "Starting location mismatch.");
        Assert(save.DayNumber == 1, "Starting day mismatch.");
        Assert(GameSaveValidator.IsValid(save), "New save should be valid.");
    }

    private static void GameSaveValidation_ReturnsErrorsForInvalidState()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        save.Player.Level = 0;
        save.DayNumber = 0;

        var errors = GameSaveValidator.Validate(save);

        Assert(errors.Count > 0, "Expected validation errors.");
        Assert(errors.Any(e => e.Contains("Player.Level", StringComparison.Ordinal)), "Missing Player.Level error.");
        Assert(errors.Any(e => e.Contains("DayNumber", StringComparison.Ordinal)), "Missing DayNumber error.");
    }

    private static void SaveLoadJson_RoundTripsGameSave()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        save.CurrentLocation = "character";
        save.RemainingDailyActions = 19;

        var json = GameSaveSerializer.SaveToJson(save);
        var roundTrip = GameSaveSerializer.LoadFromJson(json);

        Assert(roundTrip.Player.Name == save.Player.Name, "Round-trip player name mismatch.");
        Assert(roundTrip.CurrentLocation == save.CurrentLocation, "Round-trip location mismatch.");
        Assert(roundTrip.RemainingDailyActions == save.RemainingDailyActions, "Round-trip actions mismatch.");
    }

    private static void ViewModels_AreGeneratedFromGameSave()
    {
        var save = GameSaveFactory.CreateNew("Hero");
        save.Player.Gold = 50;
        save.CurrentLocation = "character";

        var screen = new ScreenManager().CreateForCurrentLocation(save);

        if (screen is not CharacterScreenViewModel characterScreen)
        {
            throw new InvalidOperationException("Expected character screen.");
        }

        Assert(characterScreen.PlayerStatus.Name == "Hero", "ViewModel player name mismatch.");
        Assert(characterScreen.Gold == 50, "ViewModel gold mismatch.");
        Assert(characterScreen.PlayerStatus.CurrentLocation == "character", "ViewModel location mismatch.");
    }

    private static void Navigation_MutatesCurrentLocationInGameSave()
    {
        var manager = new GameManager();
        var save = GameSaveFactory.CreateNew("Hero");

        manager.Execute(save, new NavigateCommand("character"));
        Assert(save.CurrentLocation == "character", "Expected navigation to character.");

        manager.Execute(save, new NavigateCommand("town"));
        Assert(save.CurrentLocation == "town", "Expected navigation to town.");
    }

    private static void Managers_DoNotRequireMutableConstructorState()
    {
        var gameManager = new GameManager();
        var screenManager = new ScreenManager();
        var playerManager = new PlayerManager();

        var save = gameManager.StartNewGame(new StartNewGameCommand("Hero"));
        var screen = screenManager.CreateForCurrentLocation(save);
        var status = playerManager.CreateStatus(save);

        Assert(screen is not null, "Expected screen output.");
        Assert(status is not null, "Expected status output.");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
