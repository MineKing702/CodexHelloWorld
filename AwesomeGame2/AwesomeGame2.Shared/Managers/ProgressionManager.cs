using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class ProgressionManager
    {
        public const int DailyActionLimit = 3;

        public void AddExperience(GameSave save, int gainedExperience)
        {
            save.Player.Experience += gainedExperience;

            while (save.Player.Experience >= ExperienceToNextLevel(save.Player.Level))
            {
                save.Player.Experience -= ExperienceToNextLevel(save.Player.Level);
                save.Player.Level += 1;
                save.Player.MaxHealth += 8;
                save.Player.Health = save.Player.MaxHealth;
                save.Player.BaseAttack += 2;
                save.Player.BaseDefense += 1;
            }
        }

        public void StartNextDay(GameSave save)
        {
            save.DayNumber += 1;
            save.RemainingDailyActions = DailyActionLimit;
            save.Player.Health = save.Player.MaxHealth;
        }

        private static int ExperienceToNextLevel(int level)
        {
            return 20 + (level - 1) * 15;
        }
    }
}
