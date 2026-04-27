using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Validation;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers;

public sealed class GameManager
{
    public GameSave StartNewGame(StartNewGameCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return GameSaveFactory.CreateNew(command.PlayerName);
    }

    public void Execute(GameSave save, GameCommand command)
    {
        ArgumentNullException.ThrowIfNull(save);
        ArgumentNullException.ThrowIfNull(command);

        switch (command)
        {
            case NavigateCommand navigate:
                ApplyNavigation(save, navigate.Destination);
                break;
            case ShowCharacterCommand:
                ApplyNavigation(save, "character");
                break;
            case StartNewGameCommand:
                throw new InvalidOperationException("Use StartNewGame for StartNewGameCommand.");
            default:
                throw new NotSupportedException($"Unsupported command type: {command.GetType().Name}");
        }

        var errors = GameSaveValidator.Validate(save);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException($"GameSave invalid after command execution: {string.Join("; ", errors)}");
        }
    }

    public ScreenViewModel BuildCurrentScreen(GameSave save)
    {
        ArgumentNullException.ThrowIfNull(save);
        return new ScreenManager().CreateForCurrentLocation(save);
    }

    private static void ApplyNavigation(GameSave save, string destination)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        save.CurrentLocation = destination.Trim().ToLowerInvariant() switch
        {
            "town" => "town",
            "character" => "character",
            _ => save.CurrentLocation
        };
    }
}
