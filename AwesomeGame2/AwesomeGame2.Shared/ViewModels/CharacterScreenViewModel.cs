namespace AwesomeGame2.Shared.ViewModels;

public sealed record CharacterScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    int Experience,
    int Gold,
    string? WeaponName,
    string? ArmorName,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("character", "Character", PlayerStatus, MenuOptions, Toast);
