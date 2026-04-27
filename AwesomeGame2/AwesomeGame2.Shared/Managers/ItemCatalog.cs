using System;
using System.Collections.Generic;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class ItemCatalog
    {
        private readonly Dictionary<string, ItemDefinition> _items;

        public ItemCatalog()
        {
            _items = new Dictionary<string, ItemDefinition>(StringComparer.Ordinal)
            {
                ["rusty_sword"] = new ItemDefinition("rusty_sword", "Rusty Sword", ItemType.Weapon, 0, 0, 0),
                ["iron_sword"] = new ItemDefinition("iron_sword", "Iron Sword", ItemType.Weapon, 30, 3, 0),
                ["steel_blade"] = new ItemDefinition("steel_blade", "Steel Blade", ItemType.Weapon, 60, 6, 0),
                ["cloth_armor"] = new ItemDefinition("cloth_armor", "Cloth Armor", ItemType.Armor, 0, 0, 0),
                ["leather_armor"] = new ItemDefinition("leather_armor", "Leather Armor", ItemType.Armor, 25, 0, 2),
                ["chain_armor"] = new ItemDefinition("chain_armor", "Chain Armor", ItemType.Armor, 55, 0, 4)
            };
        }

        public ItemDefinition GetById(string itemId)
        {
            if (!_items.ContainsKey(itemId))
            {
                throw new InvalidOperationException("Unknown item: " + itemId);
            }

            return _items[itemId];
        }

        public IReadOnlyList<ItemDefinition> GetShopInventory(string shopId)
        {
            if (string.Equals(shopId, "weapon_shop", StringComparison.Ordinal))
            {
                return new List<ItemDefinition> { _items["iron_sword"], _items["steel_blade"] };
            }

            if (string.Equals(shopId, "armor_shop", StringComparison.Ordinal))
            {
                return new List<ItemDefinition> { _items["leather_armor"], _items["chain_armor"] };
            }

            throw new InvalidOperationException("Unknown shop: " + shopId);
        }

        public string GetShopName(string shopId)
        {
            return shopId switch
            {
                "weapon_shop" => "Weapon Shop",
                "armor_shop" => "Armor Shop",
                _ => "Shop"
            };
        }
    }

    public enum ItemType
    {
        Weapon,
        Armor
    }

    public sealed record ItemDefinition(string ItemId, string Name, ItemType Type, int Price, int AttackBonus, int DefenseBonus);
}
