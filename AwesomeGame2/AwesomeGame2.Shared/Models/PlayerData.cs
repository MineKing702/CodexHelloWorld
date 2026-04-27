namespace AwesomeGame2.Shared.Models;

public sealed class PlayerData
{
    public required string Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Gold { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
}
