using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Managers;
using AwesomeGame2.Shared.Results;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Serialization;
using AwesomeGame2.Shared.ViewModels;

var manager = new GameManager();
var screenManager = new ScreenManager();
var save = manager.StartNewGame(new StartNewGameCommand("ConsoleHero"));

Console.WriteLine("AwesomeGame2 Console - Stage 3");
Console.WriteLine("Type a command id, a menu number, :save <path>, :load <path>, or :quit.");

while (true)
{
    var screen = manager.BuildCurrentScreen(save);
    RenderScreen(screen);

    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (string.Equals(input, ":quit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (TryHandleSaveLoad(input, save, screenManager))
    {
        continue;
    }

    var commandId = ResolveCommandId(input, screen.MenuOptions);
    if (commandId is null)
    {
        var invalid = screenManager.CreateErrorScreen(save, "Unknown input.", new ToastViewModel("Use a menu number or command id."));
        RenderScreen(invalid);
        continue;
    }

    var result = manager.Execute(save, new MenuCommand(commandId));
    RenderResult(result);
}

static bool TryHandleSaveLoad(string input, GameSave save, ScreenManager screenManager)
{
    if (input.StartsWith(":save ", StringComparison.OrdinalIgnoreCase))
    {
        var path = input[6..].Trim();
        try
        {
            GameSaveSerializer.SaveToFile(save, path);
            RenderScreen(screenManager.CreateSaveLoadResultScreen(save, true, "Save", path, new ToastViewModel("Save complete.")));
        }
        catch (Exception ex)
        {
            RenderScreen(screenManager.CreateSaveLoadResultScreen(save, false, "Save", path, new ToastViewModel(ex.Message)));
        }

        return true;
    }

    if (input.StartsWith(":load ", StringComparison.OrdinalIgnoreCase))
    {
        var path = input[6..].Trim();
        try
        {
            var loaded = GameSaveSerializer.LoadFromFile(path);
            CopyState(loaded, save);
            RenderScreen(screenManager.CreateSaveLoadResultScreen(save, true, "Load", path, new ToastViewModel("Load complete.")));
        }
        catch (Exception ex)
        {
            RenderScreen(screenManager.CreateSaveLoadResultScreen(save, false, "Load", path, new ToastViewModel(ex.Message)));
        }

        return true;
    }

    return false;
}

static string? ResolveCommandId(string input, IReadOnlyList<MenuOptionViewModel> options)
{
    if (int.TryParse(input, out var index) && index >= 1 && index <= options.Count)
    {
        return options[index - 1].CommandId;
    }

    if (options.Any(o => string.Equals(o.CommandId, input, StringComparison.OrdinalIgnoreCase)))
    {
        return input;
    }

    return null;
}

static void RenderResult(GameCommandResult result)
{
    RenderScreen(result.Screen);
    foreach (var message in result.Messages)
    {
        Console.WriteLine($" * {message}");
    }

    if (!string.IsNullOrWhiteSpace(result.Error))
    {
        Console.WriteLine($"Error: {result.Error}");
    }
}

static void RenderScreen(ScreenViewModel screen)
{
    Console.WriteLine();
    Console.WriteLine($"=== {screen.Title} ({screen.ScreenId}) ===");
    Console.WriteLine($"Player: {screen.PlayerStatus.Name} Lv {screen.PlayerStatus.Level}");
    Console.WriteLine($"HP: {screen.PlayerStatus.Health}/{screen.PlayerStatus.MaxHealth}  Day: {screen.PlayerStatus.DayNumber}  Actions: {screen.PlayerStatus.RemainingDailyActions}");

    switch (screen)
    {
        case CharacterScreenViewModel character:
            Console.WriteLine($"XP: {character.Experience}  Gold: {character.Gold}");
            Console.WriteLine($"Weapon: {character.WeaponName ?? "None"}  Armor: {character.ArmorName ?? "None"}");
            break;
        case BattleScreenViewModel battle:
            Console.WriteLine($"Enemy: {battle.EnemyName} {battle.EnemyHealth}/{battle.EnemyMaxHealth}");
            Console.WriteLine(battle.FlavorText);
            break;
        case LevelUpScreenViewModel levelUp:
            Console.WriteLine($"You reached level {levelUp.NewLevel}!");
            break;
        case ErrorScreenViewModel error:
            Console.WriteLine($"Invalid command: {error.ErrorMessage}");
            break;
        case SaveLoadResultScreenViewModel saveLoad:
            Console.WriteLine($"{saveLoad.Operation} {(saveLoad.Success ? "succeeded" : "failed")}: {saveLoad.Path}");
            break;
        case DeathScreenViewModel:
            Console.WriteLine("You have fallen in battle.");
            break;
    }

    if (screen.Toast is not null)
    {
        Console.WriteLine($"[Note] {screen.Toast.Message}");
    }

    Console.WriteLine("Options:");
    for (var i = 0; i < screen.MenuOptions.Count; i++)
    {
        var option = screen.MenuOptions[i];
        Console.WriteLine($" {i + 1}. {option.Label} ({option.CommandId})");
    }

    Console.WriteLine("Special: :save <path> | :load <path> | :quit");
}

static void CopyState(GameSave source, GameSave destination)
{
    destination.SaveVersion = source.SaveVersion;
    destination.Player = source.Player;
    destination.CurrentLocation = source.CurrentLocation;
    destination.DayNumber = source.DayNumber;
    destination.RemainingDailyActions = source.RemainingDailyActions;
    destination.Inventory = source.Inventory;
    destination.Equipment = source.Equipment;
    destination.ActiveBattleState = source.ActiveBattleState;
    destination.ProgressionFlags = source.ProgressionFlags;
    destination.RandomState = source.RandomState;
}
