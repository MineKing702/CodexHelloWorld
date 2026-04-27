namespace AwesomeGame2.Shared.ViewModels;

public sealed record SaveLoadResultScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    bool Success,
    string Operation,
    string Path,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("save_load_result", "Save/Load Result", PlayerStatus, MenuOptions, Toast);
