using System;
using System.Collections.Generic;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class ScreenManager
    {
        private readonly PlayerManager _playerManager;
        private readonly ShopManager _shopManager;
        private readonly InventoryManager _inventoryManager;

        public ScreenManager(PlayerManager playerManager, ShopManager shopManager, InventoryManager inventoryManager)
        {
            _playerManager = playerManager;
            _shopManager = shopManager;
            _inventoryManager = inventoryManager;
        }

        public ScreenViewModel CreateForCurrentLocation(GameSave save, ToastViewModel toast)
        {
            var status = _playerManager.CreateStatus(save);

            if (save.ActiveBattleState != null || save.CurrentLocation == "battle")
            {
                var battle = save.ActiveBattleState;
                return new BattleScreenViewModel(
                    status,
                    battle == null ? "Unknown" : battle.EnemyName,
                    battle == null ? 0 : battle.EnemyHealth,
                    battle == null ? 0 : battle.EnemyMaxHealth,
                    battle == null ? "No battle." : battle.LastBattleMessage,
                    new List<MenuOptionViewModel>
                    {
                        new MenuOptionViewModel("attack", "Attack"),
                        new MenuOptionViewModel("flee", "Flee")
                    },
                    toast);
            }

            if (save.CurrentLocation == "forest")
            {
                return new ForestScreenViewModel(
                    status,
                    "Search for enemies or return to town.",
                    new List<MenuOptionViewModel>
                    {
                        new MenuOptionViewModel("search", "Search Forest"),
                        new MenuOptionViewModel("return", "Return to Town")
                    },
                    toast);
            }

            if (save.CurrentLocation == "weapon_shop" || save.CurrentLocation == "armor_shop")
            {
                return _shopManager.BuildShopViewModel(save, save.CurrentLocation, status, toast);
            }

            if (save.CurrentLocation == "character")
            {
                return new CharacterScreenViewModel(
                    status,
                    _inventoryManager.BuildInventoryLines(save),
                    new List<MenuOptionViewModel>
                    {
                        new MenuOptionViewModel("equip", "Equip Item"),
                        new MenuOptionViewModel("return", "Return to Town")
                    },
                    toast);
            }

            return new TownScreenViewModel(
                status,
                new List<MenuOptionViewModel>
                {
                    new MenuOptionViewModel("forest", "Enter Forest"),
                    new MenuOptionViewModel("weapon_shop", "Visit Weapon Shop"),
                    new MenuOptionViewModel("armor_shop", "Visit Armor Shop"),
                    new MenuOptionViewModel("character", "View Character"),
                    new MenuOptionViewModel("end_day", "Rest / End Day")
                },
                toast);
        }
    }
}
