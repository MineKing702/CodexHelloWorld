namespace AwesomeGame2.Shared.ViewModels
{
    public sealed record ShopItemViewModel(string ItemId, string Name, int Price, int AttackBonus, int DefenseBonus, bool Owned, bool Equipped);
}
