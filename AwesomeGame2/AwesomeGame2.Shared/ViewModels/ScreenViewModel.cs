using System.Collections.Generic;

namespace AwesomeGame2.Shared.ViewModels
{
    public abstract record ScreenViewModel(
        string Title,
        PlayerStatusViewModel PlayerStatus,
        IReadOnlyList<MenuOptionViewModel> MenuOptions,
        ToastViewModel Toast);
}
