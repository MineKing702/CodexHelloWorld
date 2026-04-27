using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Content;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers;

public sealed class ScreenManager
{
    public ScreenViewModel CreateForCurrentLocation(GameSave save, ToastViewModel? toast = null)
    {
        ArgumentNullException.ThrowIfNull(save);

        return save.CurrentLocation switch
        {
            "main_menu" => CreateMainMenuScreen(save, toast),
            "character" => CreateCharacterScreen(save, toast),
            "battle" => CreateBattleScreen(save, toast),
            "level_up" => CreateLevelUpScreen(save, toast),
            "death" => CreateDeathScreen(save, toast),
            "error" => CreateErrorScreen(save, "Unknown command.", toast),
            _ => CreateTownScreen(save, toast)
        };
    }

    public MainMenuScreenViewModel CreateMainMenuScreen(GameSave save, ToastViewModel? toast = null)
        => new(new PlayerManager().CreateStatus(save),
        [new MenuOptionViewModel(CommandIds.NewGame, "Enter Town"), new MenuOptionViewModel(CommandIds.ShowCharacter, "Character")], toast);

    public TownScreenViewModel CreateTownScreen(GameSave save, ToastViewModel? toast = null)
        => new(new PlayerManager().CreateStatus(save),
        [
            new MenuOptionViewModel(CommandIds.Adventure, "Adventure"),
            new MenuOptionViewModel(CommandIds.Rest, "Rest (new day)"),
            new MenuOptionViewModel(CommandIds.ShowCharacter, "Character"),
            new MenuOptionViewModel(CommandIds.GoMainMenu, "Main Menu")
        ], toast);

    public CharacterScreenViewModel CreateCharacterScreen(GameSave save, ToastViewModel? toast = null)
    {
        var weaponName = save.Equipment.WeaponId is null ? null : GameContent.Weapons[save.Equipment.WeaponId].Name;
        var armorName = save.Equipment.ArmorId is null ? null : GameContent.Armor[save.Equipment.ArmorId].Name;

        return new CharacterScreenViewModel(
            new PlayerManager().CreateStatus(save),
            save.Player.Experience,
            save.Player.Gold,
            weaponName,
            armorName,
            [new MenuOptionViewModel(CommandIds.GoTown, "Back to Town")],
            toast);
    }

    public BattleScreenViewModel CreateBattleScreen(GameSave save, ToastViewModel? toast = null)
    {
        var battle = save.ActiveBattleState ?? throw new InvalidOperationException("Battle screen requested without active battle.");
        var enemy = GameContent.Enemies.First(e => e.Id == battle.EnemyId);

        return new BattleScreenViewModel(
            new PlayerManager().CreateStatus(save),
            battle.EnemyName,
            battle.EnemyHealth,
            battle.EnemyMaxHealth,
            enemy.FlavorText,
            [new MenuOptionViewModel(CommandIds.Attack, "Attack"), new MenuOptionViewModel(CommandIds.Flee, "Flee")],
            toast);
    }

    public LevelUpScreenViewModel CreateLevelUpScreen(GameSave save, ToastViewModel? toast = null)
        => new(new PlayerManager().CreateStatus(save), save.Player.Level, [new MenuOptionViewModel(CommandIds.AcknowledgeLevelUp, "Continue")], toast);

    public DeathScreenViewModel CreateDeathScreen(GameSave save, ToastViewModel? toast = null)
        => new(new PlayerManager().CreateStatus(save), [new MenuOptionViewModel(CommandIds.NewGame, "Try Again")], toast);

    public ErrorScreenViewModel CreateErrorScreen(GameSave save, string message, ToastViewModel? toast = null)
        => new(new PlayerManager().CreateStatus(save), message, [new MenuOptionViewModel(CommandIds.AcknowledgeError, "Continue")], toast);

    public SaveLoadResultScreenViewModel CreateSaveLoadResultScreen(GameSave save, bool success, string operation, string path, ToastViewModel? toast = null)
        => new(new PlayerManager().CreateStatus(save), success, operation, path, [new MenuOptionViewModel(CommandIds.GoMainMenu, "Main Menu")], toast);
}
