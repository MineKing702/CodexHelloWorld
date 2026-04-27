using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeGame2.Shared.Models;
using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class InventoryManager
    {
        private readonly ItemCatalog _catalog;

        public InventoryManager(ItemCatalog catalog)
        {
            _catalog = catalog;
        }

        public bool HasItem(GameSave save, string itemId)
        {
            return save.Inventory.Any(i => i.ItemId == itemId && i.Quantity > 0);
        }

        public void AddItem(GameSave save, string itemId, int quantity)
        {
            if (quantity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            var existing = save.Inventory.FirstOrDefault(i => i.ItemId == itemId);
            if (existing == null)
            {
                var def = _catalog.GetById(itemId);
                save.Inventory.Add(new InventoryItem { ItemId = itemId, DisplayName = def.Name, Quantity = quantity });
                return;
            }

            existing.Quantity += quantity;
        }

        public void Equip(GameSave save, string itemId)
        {
            if (!HasItem(save, itemId))
            {
                throw new InvalidOperationException("Item is not owned.");
            }

            var def = _catalog.GetById(itemId);
            if (def.Type == ItemType.Weapon)
            {
                save.Equipment.WeaponId = itemId;
            }
            else
            {
                save.Equipment.ArmorId = itemId;
            }
        }

        public int GetAttack(GameSave save)
        {
            var weaponBonus = 0;
            if (!string.IsNullOrWhiteSpace(save.Equipment.WeaponId))
            {
                weaponBonus = _catalog.GetById(save.Equipment.WeaponId).AttackBonus;
            }

            return save.Player.BaseAttack + weaponBonus;
        }

        public int GetDefense(GameSave save)
        {
            var armorBonus = 0;
            if (!string.IsNullOrWhiteSpace(save.Equipment.ArmorId))
            {
                armorBonus = _catalog.GetById(save.Equipment.ArmorId).DefenseBonus;
            }

            return save.Player.BaseDefense + armorBonus;
        }

        public IReadOnlyList<string> BuildInventoryLines(GameSave save)
        {
            return save.Inventory
                .OrderBy(i => i.DisplayName, StringComparer.Ordinal)
                .Select(i => i.DisplayName + " x" + i.Quantity)
                .ToList();
        }
    }
}
