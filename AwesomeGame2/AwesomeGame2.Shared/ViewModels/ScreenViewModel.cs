namespace AwesomeGame2.Shared.ViewModels;

public abstract record ScreenViewModel(
    string ScreenId,
    string Title,
    PlayerStatusViewModel PlayerStatus,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast);
