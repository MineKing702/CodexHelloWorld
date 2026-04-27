using System.Collections.Generic;

namespace AwesomeGame2.Shared.ViewModels
{
    public sealed record ShopScreenViewModel(
        PlayerStatusViewModel PlayerStatus,
        string ShopId,
        string ShopName,
        IReadOnlyList<ShopItemViewModel> Items,
        IReadOnlyList<MenuOptionViewModel> MenuOptions,
        ToastViewModel Toast)
        : ScreenViewModel("Shop", PlayerStatus, MenuOptions, Toast);
}
