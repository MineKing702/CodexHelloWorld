using System.Collections.Generic;

namespace AwesomeGame2.Shared.ViewModels
{
    public sealed record BattleScreenViewModel(
        PlayerStatusViewModel PlayerStatus,
        string EnemyName,
        int EnemyHealth,
        int EnemyMaxHealth,
        string BattleMessage,
        IReadOnlyList<MenuOptionViewModel> MenuOptions,
        ToastViewModel Toast)
        : ScreenViewModel("Battle", PlayerStatus, MenuOptions, Toast);
}
