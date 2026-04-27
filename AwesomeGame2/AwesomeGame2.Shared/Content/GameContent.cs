using AwesomeGame2.Shared.Models;

namespace AwesomeGame2.Shared.Content;

public sealed class WeaponDefinition
{
    public WeaponDefinition(string id, string name, int attackPower)
    {
        Id = id;
        Name = name;
        AttackPower = attackPower;
    }

    public string Id { get; }
    public string Name { get; }
    public int AttackPower { get; }
}

public sealed class ArmorDefinition
{
    public ArmorDefinition(string id, string name, int armorPower)
    {
        Id = id;
        Name = name;
        ArmorPower = armorPower;
    }

    public string Id { get; }
    public string Name { get; }
    public int ArmorPower { get; }
}

public sealed class EnemyDefinition
{
    public EnemyDefinition(string id, string name, int maxHealth, int attackPower, int rewardGold, int rewardExperience, string flavorText)
    {
        Id = id;
        Name = name;
        MaxHealth = maxHealth;
        AttackPower = attackPower;
        RewardGold = rewardGold;
        RewardExperience = rewardExperience;
        FlavorText = flavorText;
    }

    public string Id { get; }
    public string Name { get; }
    public int MaxHealth { get; }
    public int AttackPower { get; }
    public int RewardGold { get; }
    public int RewardExperience { get; }
    public string FlavorText { get; }
}

public static class GameContent
{
    public static readonly IReadOnlyDictionary<string, WeaponDefinition> Weapons = new Dictionary<string, WeaponDefinition>(StringComparer.Ordinal)
    {
        ["rusty_sword"] = new("rusty_sword", "Rusty Sword", 4),
        ["oak_staff"] = new("oak_staff", "Oak Staff", 6),
        ["iron_blade"] = new("iron_blade", "Iron Blade", 8)
    };

    public static readonly IReadOnlyDictionary<string, ArmorDefinition> Armor = new Dictionary<string, ArmorDefinition>(StringComparer.Ordinal)
    {
        ["cloth_tunic"] = new("cloth_tunic", "Cloth Tunic", 1),
        ["leather_vest"] = new("leather_vest", "Leather Vest", 3),
        ["chain_shirt"] = new("chain_shirt", "Chain Shirt", 5)
    };

    public static readonly IReadOnlyList<EnemyDefinition> Enemies =
    [
        new("glitch_rat", "Glitch Rat", 14, 3, 6, 12, "A rat with keyboard crumbs in its whiskers squeaks in ANSI colors."),
        new("forum_bandit", "Forum Bandit", 18, 4, 9, 16, "A masked lurker challenges your handle in the town BBS feed."),
        new("night_daemon", "Night Daemon", 22, 6, 12, 20, "An after-hours daemon hisses from a forgotten message board node.")
    ];

    public static EnemyDefinition GetEnemyForRoll(RandomState randomState)
    {
        ArgumentNullException.ThrowIfNull(randomState);
        var random = new Random(unchecked(randomState.Seed + randomState.Calls));
        var index = random.Next(Enemies.Count);
        randomState.Calls++;
        return Enemies[index];
    }
}
