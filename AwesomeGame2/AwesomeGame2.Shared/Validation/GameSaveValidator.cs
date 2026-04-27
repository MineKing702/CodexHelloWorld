using AwesomeGame2.Shared.Content;
using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Validation;

public static class GameSaveValidator
{
    public static IReadOnlyList<string> Validate(GameSave? save)
    {
        var errors = new List<string>();

        if (save is null)
        {
            errors.Add("GameSave is required.");
            return errors;
        }

        if (save.SaveVersion <= 0 || save.SaveVersion > GameSave.CurrentSaveVersion)
        {
            errors.Add("SaveVersion is invalid.");
        }

        if (save.Player is null)
        {
            errors.Add("Player is required.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(save.Player.Name))
            {
                errors.Add("Player.Name is required.");
            }

            if (save.Player.Level < 1)
            {
                errors.Add("Player.Level must be at least 1.");
            }

            if (save.Player.Experience < 0 || save.Player.Gold < 0)
            {
                errors.Add("Player progression values are invalid.");
            }

            if (save.Player.MaxHealth < 1 || save.Player.Health < 0 || save.Player.Health > save.Player.MaxHealth)
            {
                errors.Add("Player health values are invalid.");
            }
        }

        if (string.IsNullOrWhiteSpace(save.CurrentLocation))
        {
            errors.Add("CurrentLocation is required.");
        }

        if (save.DayNumber < 1)
        {
            errors.Add("DayNumber must be at least 1.");
        }

        if (save.RemainingDailyActions is < 0 or > GameRules.DailyActionLimit)
        {
            errors.Add("RemainingDailyActions is outside allowed bounds.");
        }

        if (save.Inventory.Any(i => string.IsNullOrWhiteSpace(i.ItemId) || i.Quantity < 0))
        {
            errors.Add("Inventory contains invalid items.");
        }

        if (save.Equipment.WeaponId is not null && !GameContent.Weapons.ContainsKey(save.Equipment.WeaponId))
        {
            errors.Add("WeaponId reference is invalid.");
        }

        if (save.Equipment.ArmorId is not null && !GameContent.Armor.ContainsKey(save.Equipment.ArmorId))
        {
            errors.Add("ArmorId reference is invalid.");
        }

        if (save.ActiveBattleState is not null)
        {
            if (!GameContent.Enemies.Any(e => e.Id == save.ActiveBattleState.EnemyId))
            {
                errors.Add("Active battle enemy reference is invalid.");
            }

            if (save.ActiveBattleState.EnemyMaxHealth < 1 || save.ActiveBattleState.EnemyHealth < 0 || save.ActiveBattleState.EnemyHealth > save.ActiveBattleState.EnemyMaxHealth)
            {
                errors.Add("Active battle health values are invalid.");
            }
        }

        return errors;
    }

    public static GameSave Repair(GameSave save)
    {
        ArgumentNullException.ThrowIfNull(save);

        save.DayNumber = Math.Max(1, save.DayNumber);
        save.RemainingDailyActions = Math.Clamp(save.RemainingDailyActions, 0, GameRules.DailyActionLimit);

        if (save.Player is null)
        {
            throw new InvalidOperationException("Player cannot be repaired from null.");
        }

        save.Player.Name = string.IsNullOrWhiteSpace(save.Player.Name) ? "Hero" : save.Player.Name.Trim();
        save.Player.Level = Math.Max(1, save.Player.Level);
        save.Player.Experience = Math.Max(0, save.Player.Experience);
        save.Player.Gold = Math.Max(0, save.Player.Gold);
        save.Player.MaxHealth = Math.Max(1, save.Player.MaxHealth);
        save.Player.Health = Math.Clamp(save.Player.Health, 0, save.Player.MaxHealth);

        save.Equipment ??= new();
        if (save.Equipment.WeaponId is not null && !GameContent.Weapons.ContainsKey(save.Equipment.WeaponId))
        {
            save.Equipment.WeaponId = "rusty_sword";
        }

        if (save.Equipment.ArmorId is not null && !GameContent.Armor.ContainsKey(save.Equipment.ArmorId))
        {
            save.Equipment.ArmorId = "cloth_tunic";
        }

        if (save.ActiveBattleState is not null)
        {
            var enemy = GameContent.Enemies.FirstOrDefault(e => e.Id == save.ActiveBattleState.EnemyId);
            if (enemy is null)
            {
                save.ActiveBattleState = null;
            }
            else
            {
                save.ActiveBattleState.EnemyMaxHealth = Math.Max(1, save.ActiveBattleState.EnemyMaxHealth);
                save.ActiveBattleState.EnemyHealth = Math.Clamp(save.ActiveBattleState.EnemyHealth, 0, save.ActiveBattleState.EnemyMaxHealth);
                save.ActiveBattleState.EnemyName = enemy.Name;
            }
        }

        save.Inventory = save.Inventory
            .Where(i => !string.IsNullOrWhiteSpace(i.ItemId) && i.Quantity >= 0)
            .ToList();

        return save;
    }

    public static bool IsValid(GameSave? save) => Validate(save).Count == 0;
}
