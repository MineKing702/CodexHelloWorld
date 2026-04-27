using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Managers;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.ViewModels;

var gameManager = new GameManager();
GameSave save = gameManager.StartNewGame(new StartNewGameCommand("ConsoleHero"));

RenderScreen(gameManager.BuildCurrentScreen(save));

Console.WriteLine();
Console.WriteLine("Navigating to character screen...");
gameManager.Execute(save, new ShowCharacterCommand());
RenderScreen(gameManager.BuildCurrentScreen(save));

Console.WriteLine();
Console.WriteLine("Navigating back to town...");
gameManager.Execute(save, new NavigateCommand("town"));
RenderScreen(gameManager.BuildCurrentScreen(save));

static void RenderScreen(ScreenViewModel screen)
{
    Console.WriteLine($"=== {screen.Title} ===");
    Console.WriteLine($"Player: {screen.PlayerStatus.Name} Lv {screen.PlayerStatus.Level}");
    Console.WriteLine($"HP: {screen.PlayerStatus.Health}/{screen.PlayerStatus.MaxHealth}");
    Console.WriteLine($"Day: {screen.PlayerStatus.DayNumber} | Actions: {screen.PlayerStatus.RemainingDailyActions}");
    Console.WriteLine($"Location: {screen.PlayerStatus.CurrentLocation}");

    if (screen is CharacterScreenViewModel character)
    {
        Console.WriteLine($"Gold: {character.Gold} | XP: {character.Experience}");
    }

    Console.WriteLine("Options:");
    foreach (var option in screen.MenuOptions)
    {
        Console.WriteLine($" - [{option.CommandId}] {option.Label}");
    }
}
