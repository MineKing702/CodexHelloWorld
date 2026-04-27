namespace AwesomeGame2.Shared.ViewModels
{
    public sealed record PlayerStatusViewModel(
        string Name,
        int Level,
        int Experience,
        int Gold,
        int Health,
        int MaxHealth,
        int Attack,
        int Defense,
        int DayNumber,
        int RemainingDailyActions,
        string CurrentLocation,
        string EquippedWeapon,
        string EquippedArmor);
}
