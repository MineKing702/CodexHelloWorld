namespace AwesomeGame2.Shared.ViewModels;

public sealed record ErrorScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    string ErrorMessage,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("error", "Invalid Command", PlayerStatus, MenuOptions, Toast);
