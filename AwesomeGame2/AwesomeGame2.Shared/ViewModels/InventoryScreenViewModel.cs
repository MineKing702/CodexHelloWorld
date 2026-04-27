using System.Collections.Generic;

namespace AwesomeGame2.Shared.ViewModels
{
    public sealed record InventoryScreenViewModel(
        PlayerStatusViewModel PlayerStatus,
        IReadOnlyList<string> InventoryLines,
        IReadOnlyList<MenuOptionViewModel> MenuOptions,
        ToastViewModel Toast)
        : ScreenViewModel("Inventory", PlayerStatus, MenuOptions, Toast);
}
