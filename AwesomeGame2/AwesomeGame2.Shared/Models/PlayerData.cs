namespace AwesomeGame2.Shared.Models
{
    public sealed class PlayerData
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }

        public PlayerData()
        {
            Name = string.Empty;
            Level = 1;
            Experience = 0;
            Gold = 0;
            Health = 1;
            MaxHealth = 1;
            BaseAttack = 1;
            BaseDefense = 0;
        }
    }
}
