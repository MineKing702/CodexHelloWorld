namespace AwesomeGame2.Shared.ViewModels;

public sealed record DeathScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("death", "Defeat", PlayerStatus, MenuOptions, Toast);
