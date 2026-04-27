using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers;

public sealed class ScreenManager
{
    public TownScreenViewModel CreateTownScreen(GameSave save, ToastViewModel? toast = null)
    {
        ArgumentNullException.ThrowIfNull(save);
        var status = new PlayerManager().CreateStatus(save);

        return new TownScreenViewModel(
            status,
            [
                new MenuOptionViewModel("show_character", "Character"),
                new MenuOptionViewModel("go_town", "Town")
            ],
            toast);
    }

    public CharacterScreenViewModel CreateCharacterScreen(GameSave save, ToastViewModel? toast = null)
    {
        ArgumentNullException.ThrowIfNull(save);
        var status = new PlayerManager().CreateStatus(save);

        return new CharacterScreenViewModel(
            status,
            save.Player.Experience,
            save.Player.Gold,
            [
                new MenuOptionViewModel("go_town", "Back to Town")
            ],
            toast);
    }

    public ScreenViewModel CreateForCurrentLocation(GameSave save, ToastViewModel? toast = null)
    {
        return save.CurrentLocation switch
        {
            "character" => CreateCharacterScreen(save, toast),
            _ => CreateTownScreen(save, toast)
        };
    }
}
