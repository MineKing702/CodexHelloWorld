namespace AwesomeGame2.Shared.ViewModels;

public sealed record LevelUpScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    int NewLevel,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("level_up", "Level Up!", PlayerStatus, MenuOptions, Toast);
