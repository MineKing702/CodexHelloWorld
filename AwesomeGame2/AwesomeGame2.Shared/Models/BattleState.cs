namespace AwesomeGame2.Shared.Models
{
    public sealed class BattleState
    {
        public string EnemyId { get; set; }
        public string EnemyName { get; set; }
        public int EnemyHealth { get; set; }
        public int EnemyMaxHealth { get; set; }
        public int EnemyAttack { get; set; }
        public int EnemyDefense { get; set; }
        public int RewardGold { get; set; }
        public int RewardExperience { get; set; }
        public bool IsPlayerTurn { get; set; }
        public string LastBattleMessage { get; set; }

        public BattleState()
        {
            EnemyId = string.Empty;
            EnemyName = string.Empty;
            LastBattleMessage = string.Empty;
        }
    }
}
