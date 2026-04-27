using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class ShopManager
    {
        private readonly ItemCatalog _catalog;
        private readonly InventoryManager _inventoryManager;

        public ShopManager(ItemCatalog catalog, InventoryManager inventoryManager)
        {
            _catalog = catalog;
            _inventoryManager = inventoryManager;
        }

        public void VisitShop(GameSave save, string shopId)
        {
            save.CurrentLocation = shopId;
        }

        public string BuyItem(GameSave save, string itemId)
        {
            var item = _catalog.GetById(itemId);
            if (save.Player.Gold < item.Price)
            {
                return "Not enough gold.";
            }

            save.Player.Gold -= item.Price;
            _inventoryManager.AddItem(save, itemId, 1);
            return "Bought " + item.Name + ".";
        }

        public ShopScreenViewModel BuildShopViewModel(GameSave save, string shopId, PlayerStatusViewModel status, ToastViewModel toast)
        {
            var shopItems = _catalog.GetShopInventory(shopId);
            var list = shopItems.Select(item => new ShopItemViewModel(
                item.ItemId,
                item.Name,
                item.Price,
                item.AttackBonus,
                item.DefenseBonus,
                _inventoryManager.HasItem(save, item.ItemId),
                string.Equals(save.Equipment.WeaponId, item.ItemId, StringComparison.Ordinal) || string.Equals(save.Equipment.ArmorId, item.ItemId, StringComparison.Ordinal))).ToList();

            var menu = new List<MenuOptionViewModel>
            {
                new MenuOptionViewModel("buy", "Buy Item"),
                new MenuOptionViewModel("equip", "Equip Item"),
                new MenuOptionViewModel("return", "Return to Town")
            };

            return new ShopScreenViewModel(status, shopId, _catalog.GetShopName(shopId), list, menu, toast);
        }
    }
}
