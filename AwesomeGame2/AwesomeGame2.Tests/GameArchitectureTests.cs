using System;
using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Managers;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Serialization;
using AwesomeGame2.Shared.Validation;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Tests
{
    public static class GameArchitectureTests
    {
        public static void RunAll()
        {
            ForestSearch_ConsumesDailyAction();
            BattleState_IsStoredInGameSave();
            Attack_MutatesActiveBattleState();
            Victory_GrantsRewardsAndClearsBattle();
            BuyingEquipment_UpdatesSaveAndGold();
            EquippedItems_AffectCombatStats();
            SaveLoad_RoundTripAcrossCoreStates();
            ConsoleFlow_UsesCommandsAndViewModelsOnly();
            Save_IsAlwaysValidAfterCommands();
        }

        private static void ForestSearch_ConsumesDailyAction()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));

            manager.Execute(save, new EnterForestCommand());
            var before = save.RemainingDailyActions;
            manager.Execute(save, new SearchForestCommand());

            Assert(save.RemainingDailyActions == before - 1, "Forest search should consume one action.");
        }

        private static void BattleState_IsStoredInGameSave()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));
            save.RandomState.Seed = 1;

            manager.Execute(save, new EnterForestCommand());
            TriggerBattle(manager, save);

            Assert(save.ActiveBattleState != null, "Battle should be active in save.");
            Assert(save.CurrentLocation == "battle", "Location should be battle.");
        }

        private static void Attack_MutatesActiveBattleState()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));
            save.RandomState.Seed = 1;

            manager.Execute(save, new EnterForestCommand());
            TriggerBattle(manager, save);
            var beforeEnemyHealth = save.ActiveBattleState == null ? 0 : save.ActiveBattleState.EnemyHealth;

            manager.Execute(save, new AttackCommand());

            if (save.ActiveBattleState != null)
            {
                Assert(save.ActiveBattleState.EnemyHealth < beforeEnemyHealth, "Enemy health should drop after attack.");
            }
        }

        private static void Victory_GrantsRewardsAndClearsBattle()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));
            save.RandomState.Seed = 1;

            manager.Execute(save, new EnterForestCommand());
            TriggerBattle(manager, save);

            var startGold = save.Player.Gold;
            var startXp = save.Player.Experience;

            while (save.ActiveBattleState != null)
            {
                manager.Execute(save, new AttackCommand());
            }

            Assert(save.CurrentLocation == "forest", "Should return to forest after win.");
            Assert(save.Player.Gold > startGold, "Gold should increase on victory.");
            Assert(save.Player.Experience >= startXp, "XP should increase on victory.");
        }

        private static void BuyingEquipment_UpdatesSaveAndGold()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));

            manager.Execute(save, new VisitShopCommand("weapon_shop"));
            var beforeGold = save.Player.Gold;
            manager.Execute(save, new BuyItemCommand("iron_sword"));

            Assert(save.Player.Gold == beforeGold - 30, "Buying iron sword should reduce gold by price.");
            Assert(save.Inventory.Exists(i => i.ItemId == "iron_sword" && i.Quantity > 0), "Inventory should contain bought item.");
        }

        private static void EquippedItems_AffectCombatStats()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));
            save.Player.Gold = 999;

            manager.Execute(save, new VisitShopCommand("weapon_shop"));
            manager.Execute(save, new BuyItemCommand("steel_blade"));
            manager.Execute(save, new EquipItemCommand("steel_blade"));
            manager.Execute(save, new ReturnToTownCommand());

            var screen = manager.BuildCurrentScreen(save, new ToastViewModel("check"));
            Assert(screen.PlayerStatus.Attack >= save.Player.BaseAttack + 6, "Equipped weapon should increase attack.");
        }

        private static void SaveLoad_RoundTripAcrossCoreStates()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));
            var townJson = GameSaveSerializer.SaveToJson(save);
            var townLoaded = GameSaveSerializer.LoadFromJson(townJson);
            Assert(townLoaded.CurrentLocation == "town", "Town round-trip should persist location.");

            manager.Execute(save, new EnterForestCommand());
            var forestJson = GameSaveSerializer.SaveToJson(save);
            var forestLoaded = GameSaveSerializer.LoadFromJson(forestJson);
            Assert(forestLoaded.CurrentLocation == "forest", "Forest round-trip should persist location.");

            save.RandomState.Seed = 1;
            TriggerBattle(manager, save);
            var battleJson = GameSaveSerializer.SaveToJson(save);
            var battleLoaded = GameSaveSerializer.LoadFromJson(battleJson);
            Assert(battleLoaded.ActiveBattleState != null, "Active battle should round-trip.");

            save.Player.Gold = 999;
            manager.Execute(save, new ReturnToTownCommand());
            manager.Execute(save, new VisitShopCommand("armor_shop"));
            manager.Execute(save, new BuyItemCommand("leather_armor"));
            var boughtJson = GameSaveSerializer.SaveToJson(save);
            var boughtLoaded = GameSaveSerializer.LoadFromJson(boughtJson);
            Assert(boughtLoaded.Inventory.Exists(i => i.ItemId == "leather_armor"), "Bought equipment should round-trip.");
        }

        private static void ConsoleFlow_UsesCommandsAndViewModelsOnly()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));
            var toast = new ToastViewModel("start");
            var screen = manager.BuildCurrentScreen(save, toast);

            Assert(screen is TownScreenViewModel, "Expected town screen at start.");

            toast = manager.Execute(save, new EnterForestCommand());
            screen = manager.BuildCurrentScreen(save, toast);
            Assert(screen is ForestScreenViewModel, "Expected forest screen after enter forest.");

            manager.Execute(save, new ReturnToTownCommand());
            manager.Execute(save, new VisitShopCommand("weapon_shop"));
            screen = manager.BuildCurrentScreen(save, new ToastViewModel("shop"));
            Assert(screen is ShopScreenViewModel, "Expected shop screen after visit shop.");
        }

        private static void TriggerBattle(GameManager manager, GameSave save)
        {
            while (save.ActiveBattleState == null && save.RemainingDailyActions > 0)
            {
                manager.Execute(save, new SearchForestCommand());
            }

            if (save.ActiveBattleState == null)
            {
                throw new InvalidOperationException("Expected battle trigger for test.");
            }
        }

        private static void Save_IsAlwaysValidAfterCommands()
        {
            var manager = new GameManager();
            var save = manager.StartNewGame(new StartNewGameCommand("Hero"));
            manager.Execute(save, new EnterForestCommand());
            manager.Execute(save, new SearchForestCommand());
            if (save.ActiveBattleState != null)
            {
                manager.Execute(save, new AttackCommand());
            }

            Assert(GameSaveValidator.IsValid(save), "Save should remain valid after command sequence.");
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
