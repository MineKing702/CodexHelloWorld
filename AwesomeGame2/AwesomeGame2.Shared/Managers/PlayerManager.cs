using System;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class PlayerManager
    {
        private readonly InventoryManager _inventoryManager;

        public PlayerManager(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        public PlayerStatusViewModel CreateStatus(GameSave save)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            return new PlayerStatusViewModel(
                save.Player.Name,
                save.Player.Level,
                save.Player.Experience,
                save.Player.Gold,
                save.Player.Health,
                save.Player.MaxHealth,
                _inventoryManager.GetAttack(save),
                _inventoryManager.GetDefense(save),
                save.DayNumber,
                save.RemainingDailyActions,
                save.CurrentLocation,
                save.Equipment.WeaponId,
                save.Equipment.ArmorId);
        }
    }
}
