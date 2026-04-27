namespace AwesomeGame2.Shared.ViewModels;

public sealed record CharacterScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    int Experience,
    int Gold,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("Character", PlayerStatus, MenuOptions, Toast);
