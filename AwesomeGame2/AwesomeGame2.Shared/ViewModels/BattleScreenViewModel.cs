namespace AwesomeGame2.Shared.ViewModels;

public sealed record BattleScreenViewModel(
    PlayerStatusViewModel PlayerStatus,
    string EnemyName,
    int EnemyHealth,
    int EnemyMaxHealth,
    string FlavorText,
    IReadOnlyList<MenuOptionViewModel> MenuOptions,
    ToastViewModel? Toast)
    : ScreenViewModel("battle", "Battle", PlayerStatus, MenuOptions, Toast);
