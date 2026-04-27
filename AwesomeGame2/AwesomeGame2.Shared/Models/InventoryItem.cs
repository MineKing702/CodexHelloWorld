namespace AwesomeGame2.Shared.Models;

public sealed class InventoryItem
{
    public required string ItemId { get; set; }
    public required string DisplayName { get; set; }
    public int Quantity { get; set; }
}
