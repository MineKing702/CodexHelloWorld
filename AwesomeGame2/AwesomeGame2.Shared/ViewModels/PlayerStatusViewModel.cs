namespace AwesomeGame2.Shared.ViewModels;

public sealed record PlayerStatusViewModel(
    string Name,
    int Level,
    int Health,
    int MaxHealth,
    int DayNumber,
    int RemainingDailyActions,
    string CurrentLocation);
