using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers;

public sealed class PlayerManager
{
    public PlayerStatusViewModel CreateStatus(GameSave save)
    {
        ArgumentNullException.ThrowIfNull(save);

        return new PlayerStatusViewModel(
            save.Player.Name,
            save.Player.Level,
            save.Player.Health,
            save.Player.MaxHealth,
            save.DayNumber,
            save.RemainingDailyActions,
            save.CurrentLocation);
    }
}
