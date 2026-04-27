namespace AwesomeGame2.Shared.Models
{
    public sealed class InventoryItem
    {
        public string ItemId { get; set; }
        public string DisplayName { get; set; }
        public int Quantity { get; set; }

        public InventoryItem()
        {
            ItemId = string.Empty;
            DisplayName = string.Empty;
            Quantity = 0;
        }
    }
}
