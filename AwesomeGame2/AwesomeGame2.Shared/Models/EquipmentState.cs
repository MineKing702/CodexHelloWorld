namespace AwesomeGame2.Shared.Models
{
    public sealed class EquipmentState
    {
        public string WeaponId { get; set; }
        public string ArmorId { get; set; }

        public EquipmentState()
        {
            WeaponId = string.Empty;
            ArmorId = string.Empty;
        }
    }
}
