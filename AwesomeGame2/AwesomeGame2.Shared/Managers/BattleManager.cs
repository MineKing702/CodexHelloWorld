using System;
using System.Collections.Generic;
using AwesomeGame2.Shared.Models;
using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class BattleManager
    {
        private readonly RandomManager _randomManager;
        private readonly InventoryManager _inventoryManager;
        private readonly ProgressionManager _progressionManager;
        private readonly IReadOnlyList<EnemyDefinition> _forestEnemies;

        public BattleManager(RandomManager randomManager, InventoryManager inventoryManager, ProgressionManager progressionManager)
        {
            _randomManager = randomManager;
            _inventoryManager = inventoryManager;
            _progressionManager = progressionManager;
            _forestEnemies = new List<EnemyDefinition>
            {
                new EnemyDefinition("slime", "Green Slime", 18, 5, 1, 8, 12),
                new EnemyDefinition("wolf", "Forest Wolf", 24, 7, 2, 12, 15),
                new EnemyDefinition("boar", "Wild Boar", 28, 8, 3, 15, 18)
            };
        }

        public void StartForestBattle(GameSave save)
        {
            if (save.ActiveBattleState != null)
            {
                return;
            }

            var index = _randomManager.Next(save, 0, _forestEnemies.Count);
            var enemy = _forestEnemies[index];
            save.ActiveBattleState = new BattleState
            {
                EnemyId = enemy.Id,
                EnemyName = enemy.Name,
                EnemyHealth = enemy.Health,
                EnemyMaxHealth = enemy.Health,
                EnemyAttack = enemy.Attack,
                EnemyDefense = enemy.Defense,
                RewardGold = enemy.Gold,
                RewardExperience = enemy.Experience,
                IsPlayerTurn = true,
                LastBattleMessage = "An enemy appears: " + enemy.Name + "!"
            };

            save.CurrentLocation = "battle";
        }

        public string Attack(GameSave save)
        {
            EnsureBattle(save);
            var battle = save.ActiveBattleState;
            if (battle == null)
            {
                throw new InvalidOperationException("No active battle.");
            }

            var playerAttack = _inventoryManager.GetAttack(save);
            var dealt = Math.Max(1, playerAttack - battle.EnemyDefense);
            battle.EnemyHealth = Math.Max(0, battle.EnemyHealth - dealt);

            if (battle.EnemyHealth == 0)
            {
                save.Player.Gold += battle.RewardGold;
                _progressionManager.AddExperience(save, battle.RewardExperience);
                save.ActiveBattleState = null;
                save.CurrentLocation = "forest";
                return "Victory! You dealt " + dealt + " and earned " + battle.RewardGold + " gold / " + battle.RewardExperience + " XP.";
            }

            var retaliation = EnemyAttack(save, battle);
            battle.LastBattleMessage = "You dealt " + dealt + ". Enemy retaliates for " + retaliation + ".";
            return battle.LastBattleMessage;
        }

        public string Flee(GameSave save)
        {
            EnsureBattle(save);
            var fleeRoll = _randomManager.Next(save, 0, 100);
            if (fleeRoll < 45)
            {
                save.ActiveBattleState = null;
                save.CurrentLocation = "forest";
                return "You escaped safely.";
            }

            var battle = save.ActiveBattleState;
            if (battle == null)
            {
                throw new InvalidOperationException("No active battle.");
            }

            var retaliation = EnemyAttack(save, battle);
            battle.LastBattleMessage = "Flee failed. Enemy hits for " + retaliation + ".";
            return battle.LastBattleMessage;
        }

        private int EnemyAttack(GameSave save, BattleState battle)
        {
            var playerDefense = _inventoryManager.GetDefense(save);
            var raw = battle.EnemyAttack - playerDefense;
            var damage = Math.Max(1, raw);
            save.Player.Health = Math.Max(0, save.Player.Health - damage);

            if (save.Player.Health == 0)
            {
                save.ActiveBattleState = null;
                save.CurrentLocation = "town";
                save.Player.Health = Math.Max(1, save.Player.MaxHealth / 2);
                return damage;
            }

            return damage;
        }

        private static void EnsureBattle(GameSave save)
        {
            if (save.ActiveBattleState == null)
            {
                throw new InvalidOperationException("No active battle.");
            }
        }

        private sealed record EnemyDefinition(string Id, string Name, int Health, int Attack, int Defense, int Gold, int Experience);
    }
}
