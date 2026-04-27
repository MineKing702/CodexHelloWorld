using AwesomeGame2.Shared.Models;

namespace AwesomeGame2.Shared.Saves;

public static class GameSaveFactory
{
    public static GameSave CreateNew(string playerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(playerName);

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
                Gold = 25,
                Health = 100,
                MaxHealth = 100
            },
            CurrentLocation = "town",
            DayNumber = 1,
            RemainingDailyActions = 20,
            Inventory =
            [
                new InventoryItem
                {
                    ItemId = "basic_potion",
                    DisplayName = "Basic Potion",
                    Quantity = 1
                }
            ],
            Equipment = new EquipmentState
            {
                WeaponId = "rusty_sword",
                ArmorId = "cloth_tunic"
            },
            ActiveBattleState = null,
            ProgressionFlags = new ProgressionFlags(),
            RandomState = new RandomState
            {
                Seed = seed,
                Calls = 0
            }
        };
    }
}
