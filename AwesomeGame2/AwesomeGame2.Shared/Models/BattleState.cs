namespace AwesomeGame2.Shared.Models;

public sealed class BattleState
{
    public required string EnemyId { get; set; }
    public required string EnemyName { get; set; }
    public int EnemyHealth { get; set; }
    public int EnemyMaxHealth { get; set; }
    public bool IsPlayerTurn { get; set; }
}
