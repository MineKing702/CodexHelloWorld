namespace AwesomeGame2.Shared.ViewModels;

public sealed record TownScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("Town Square", PlayerStatus, MenuOptions, Toast);
