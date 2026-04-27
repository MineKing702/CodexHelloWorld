using System;
using System.Collections.Generic;
using AwesomeGame2.Shared.Models;

namespace AwesomeGame2.Shared.Saves
{
    public static class GameSaveFactory
    {
        public static GameSave CreateNew(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentException("Player name is required.", nameof(playerName));
            }

            var cleanName = playerName.Trim();
            var seed = cleanName.GetHashCode(StringComparison.Ordinal);

            return new GameSave
            {
                SaveVersion = GameSave.CurrentSaveVersion,
                Player = new PlayerData
                {
                    Name = cleanName,
                    Level = 1,
                    Experience = 0,
                    Gold = 30,
                    Health = 40,
                    MaxHealth = 40,
                    BaseAttack = 6,
                    BaseDefense = 2
                },
                CurrentLocation = "town",
                DayNumber = 1,
                RemainingDailyActions = 3,
                Inventory = new List<InventoryItem>
                {
                    new InventoryItem { ItemId = "rusty_sword", DisplayName = "Rusty Sword", Quantity = 1 },
                    new InventoryItem { ItemId = "cloth_armor", DisplayName = "Cloth Armor", Quantity = 1 }
                },
                Equipment = new EquipmentState
                {
                    WeaponId = "rusty_sword",
                    ArmorId = "cloth_armor"
                },
                ActiveBattleState = null,
                ProgressionFlags = new ProgressionFlags(),
                RandomState = new RandomState { Seed = seed, Calls = 0 }
            };
        }
    }
}
