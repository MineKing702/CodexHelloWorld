using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Content;
using AwesomeGame2.Shared.Models;
using AwesomeGame2.Shared.Results;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Serialization;
using AwesomeGame2.Shared.Validation;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers;

public sealed class GameManager
{
    private readonly ScreenManager _screenManager = new();

    public GameSave StartNewGame(StartNewGameCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return GameSaveFactory.CreateNew(command.PlayerName);
    }

    public GameCommandResult Execute(GameSave save, GameCommand command)
    {
        ArgumentNullException.ThrowIfNull(save);
        ArgumentNullException.ThrowIfNull(command);

        var snapshot = GameSaveSerializer.SaveToJson(save);
        try
        {
            var messages = ApplyCommand(save, command);
            var errors = GameSaveValidator.Validate(save);
            if (errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join("; ", errors));
            }

            return new GameCommandResult(_screenManager.CreateForCurrentLocation(save), messages, true, null);
        }
        catch (Exception ex)
        {
            var reverted = GameSaveSerializer.LoadFromJson(snapshot);
            CopyState(reverted, save);

            save.CurrentLocation = "error";
            var errorScreen = _screenManager.CreateErrorScreen(save, ex.Message, new ToastViewModel("No changes were applied."));
            return new GameCommandResult(errorScreen, ["Command rejected."], false, ex.Message);
        }
    }

    public ScreenViewModel BuildCurrentScreen(GameSave save)
    {
        ArgumentNullException.ThrowIfNull(save);
        return _screenManager.CreateForCurrentLocation(save);
    }

    private static List<string> ApplyCommand(GameSave save, GameCommand command)
    {
        return command switch
        {
            MenuCommand menu => ApplyMenu(save, menu),
            StartNewGameCommand => throw new InvalidOperationException("Use StartNewGame for StartNewGameCommand."),
            _ => throw new NotSupportedException($"Unsupported command type: {command.GetType().Name}")
        };
    }

    private static List<string> ApplyMenu(GameSave save, MenuCommand menu)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(menu.CommandId);

        var messages = new List<string>();
        switch (menu.CommandId)
        {
            case CommandIds.NewGame:
                save.CurrentLocation = "town";
                save.Player.Health = save.Player.MaxHealth;
                save.ActiveBattleState = null;
                messages.Add("You enter the square where rumor boards glow in phosphor green.");
                break;
            case CommandIds.ShowCharacter:
                save.CurrentLocation = "character";
                break;
            case CommandIds.GoTown:
                save.CurrentLocation = "town";
                break;
            case CommandIds.GoMainMenu:
                save.CurrentLocation = "main_menu";
                break;
            case CommandIds.Adventure:
                StartAdventure(save, messages);
                break;
            case CommandIds.Attack:
                ResolveAttack(save, messages);
                break;
            case CommandIds.Flee:
                save.ActiveBattleState = null;
                save.CurrentLocation = "town";
                messages.Add("You retreat to town before the modem line drops.");
                break;
            case CommandIds.Rest:
                save.DayNumber++;
                save.RemainingDailyActions = GameRules.DailyActionLimit;
                save.Player.Health = save.Player.MaxHealth;
                save.CurrentLocation = "town";
                messages.Add("A new day begins. Daily actions reset.");
                break;
            case CommandIds.AcknowledgeLevelUp:
                save.CurrentLocation = "town";
                messages.Add("You feel stronger.");
                break;
            case CommandIds.AcknowledgeError:
                save.CurrentLocation = "town";
                break;
            default:
                throw new InvalidOperationException($"Unknown command id '{menu.CommandId}'.");
        }

        return messages;
    }

    private static void StartAdventure(GameSave save, List<string> messages)
    {
        if (save.RemainingDailyActions <= 0)
        {
            throw new InvalidOperationException("You are out of daily actions. Rest to continue tomorrow.");
        }

        if (save.ActiveBattleState is not null)
        {
            save.CurrentLocation = "battle";
            messages.Add("Battle resumed.");
            return;
        }

        save.RemainingDailyActions--;
        var enemy = GameContent.GetEnemyForRoll(save.RandomState);
        save.ActiveBattleState = new BattleState
        {
            EnemyId = enemy.Id,
            EnemyName = enemy.Name,
            EnemyHealth = enemy.MaxHealth,
            EnemyMaxHealth = enemy.MaxHealth,
            IsPlayerTurn = true
        };

        save.CurrentLocation = "battle";
        messages.Add($"A {enemy.Name} appears!");
    }

    private static void ResolveAttack(GameSave save, List<string> messages)
    {
        var battle = save.ActiveBattleState ?? throw new InvalidOperationException("No active battle.");
        var enemyDef = GameContent.Enemies.First(e => e.Id == battle.EnemyId);
        var weaponPower = save.Equipment.WeaponId is null ? 1 : GameContent.Weapons[save.Equipment.WeaponId].AttackPower;
        var armorPower = save.Equipment.ArmorId is null ? 0 : GameContent.Armor[save.Equipment.ArmorId].ArmorPower;

        battle.EnemyHealth = Math.Max(0, battle.EnemyHealth - weaponPower);
        messages.Add($"You hit {battle.EnemyName} for {weaponPower} damage.");

        if (battle.EnemyHealth == 0)
        {
            save.Player.Gold += enemyDef.RewardGold;
            save.Player.Experience += enemyDef.RewardExperience;
            messages.Add($"Victory! +{enemyDef.RewardGold} gold, +{enemyDef.RewardExperience} xp.");
            save.ActiveBattleState = null;

            var previousLevel = save.Player.Level;
            var targetLevel = (save.Player.Experience / GameRules.XpPerLevel) + 1;
            if (targetLevel > save.Player.Level)
            {
                save.Player.Level = targetLevel;
                save.Player.MaxHealth += (targetLevel - previousLevel) * 5;
                save.Player.Health = save.Player.MaxHealth;
                save.CurrentLocation = "level_up";
                messages.Add($"Level up! You are now level {save.Player.Level}.");
            }
            else
            {
                save.CurrentLocation = "town";
            }

            return;
        }

        var enemyDamage = Math.Max(1, enemyDef.AttackPower - armorPower);
        save.Player.Health = Math.Max(0, save.Player.Health - enemyDamage);
        messages.Add($"{battle.EnemyName} hits back for {enemyDamage}.");

        if (save.Player.Health == 0)
        {
            save.ActiveBattleState = null;
            save.CurrentLocation = "death";
            messages.Add("You were defeated.");
        }
        else
        {
            save.CurrentLocation = "battle";
        }
    }

    private static void CopyState(GameSave source, GameSave destination)
    {
        destination.SaveVersion = source.SaveVersion;
        destination.Player = source.Player;
        destination.CurrentLocation = source.CurrentLocation;
        destination.DayNumber = source.DayNumber;
        destination.RemainingDailyActions = source.RemainingDailyActions;
        destination.Inventory = source.Inventory;
        destination.Equipment = source.Equipment;
        destination.ActiveBattleState = source.ActiveBattleState;
        destination.ProgressionFlags = source.ProgressionFlags;
        destination.RandomState = source.RandomState;
    }
}
