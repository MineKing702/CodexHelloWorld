using System.Collections.Generic;

namespace AwesomeGame2.Shared.ViewModels
{
    public sealed record ForestScreenViewModel(
        PlayerStatusViewModel PlayerStatus,
        string ForestSummary,
        IReadOnlyList<MenuOptionViewModel> MenuOptions,
        ToastViewModel Toast)
        : ScreenViewModel("Forest", PlayerStatus, MenuOptions, Toast);
}
