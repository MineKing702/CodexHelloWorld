namespace AwesomeGame2.Shared.ViewModels;

public sealed record MainMenuScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("main_menu", "Main Menu", PlayerStatus, MenuOptions, Toast);
