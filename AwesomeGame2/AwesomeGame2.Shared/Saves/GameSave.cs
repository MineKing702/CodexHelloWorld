using System.Collections.Generic;
using AwesomeGame2.Shared.Models;

namespace AwesomeGame2.Shared.Saves
{
    public sealed class GameSave
    {
        public const int CurrentSaveVersion = 1;

        public int SaveVersion { get; set; }
        public PlayerData Player { get; set; }
        public string CurrentLocation { get; set; }
        public int DayNumber { get; set; }
        public int RemainingDailyActions { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public EquipmentState Equipment { get; set; }
        public BattleState? ActiveBattleState { get; set; }
        public ProgressionFlags ProgressionFlags { get; set; }
        public RandomState RandomState { get; set; }

        public GameSave()
        {
            SaveVersion = CurrentSaveVersion;
            Player = new PlayerData();
            CurrentLocation = "town";
            DayNumber = 1;
            RemainingDailyActions = 0;
            Inventory = new List<InventoryItem>();
            Equipment = new EquipmentState();
            ActiveBattleState = null;
            ProgressionFlags = new ProgressionFlags();
            RandomState = new RandomState();
        }
    }
}
