using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class WorldManager
    {
        private readonly RandomManager _randomManager;
        private readonly BattleManager _battleManager;
        private readonly ProgressionManager _progressionManager;

        public WorldManager(RandomManager randomManager, BattleManager battleManager, ProgressionManager progressionManager)
        {
            _randomManager = randomManager;
            _battleManager = battleManager;
            _progressionManager = progressionManager;
        }

        public void EnterForest(GameSave save)
        {
            save.CurrentLocation = "forest";
        }

        public string SearchForest(GameSave save)
        {
            if (save.RemainingDailyActions <= 0)
            {
                return "No actions remaining. End day in town.";
            }

            save.RemainingDailyActions -= 1;
            var encounterRoll = _randomManager.Next(save, 0, 100);
            if (encounterRoll < 70)
            {
                _battleManager.StartForestBattle(save);
                return "You found trouble in the woods.";
            }

            var foundGold = _randomManager.Next(save, 3, 11);
            save.Player.Gold += foundGold;
            return "Quiet search. You found " + foundGold + " gold.";
        }

        public string EndDay(GameSave save)
        {
            if (save.CurrentLocation != "town")
            {
                return "Return to town before resting.";
            }

            _progressionManager.StartNextDay(save);
            return "A new day begins.";
        }

        public void ReturnToTown(GameSave save)
        {
            save.CurrentLocation = "town";
            save.ActiveBattleState = null;
        }
    }
}
