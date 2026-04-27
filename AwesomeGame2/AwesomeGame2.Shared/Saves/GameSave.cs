using AwesomeGame2.Shared.Models;

namespace AwesomeGame2.Shared.Saves;

public sealed class GameSave
{
    public const int CurrentSaveVersion = 1;

    public int SaveVersion { get; set; } = CurrentSaveVersion;
    public required PlayerData Player { get; set; }
    public required string CurrentLocation { get; set; }
    public int DayNumber { get; set; }
    public int RemainingDailyActions { get; set; }
    public List<InventoryItem> Inventory { get; set; } = [];
    public EquipmentState Equipment { get; set; } = new();
    public BattleState? ActiveBattleState { get; set; }
    public ProgressionFlags ProgressionFlags { get; set; } = new();
    public RandomState RandomState { get; set; } = new();
}
