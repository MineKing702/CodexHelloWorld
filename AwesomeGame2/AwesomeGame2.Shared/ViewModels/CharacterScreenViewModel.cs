using System.Collections.Generic;

namespace AwesomeGame2.Shared.ViewModels
{
    public sealed record CharacterScreenViewModel(
        PlayerStatusViewModel PlayerStatus,
        IReadOnlyList<string> InventoryLines,
        IReadOnlyList<MenuOptionViewModel> MenuOptions,
        ToastViewModel Toast)
        : ScreenViewModel("Character", PlayerStatus, MenuOptions, Toast);
}
