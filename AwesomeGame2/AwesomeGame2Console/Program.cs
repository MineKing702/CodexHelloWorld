using System;
using System.Linq;
using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Managers;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Serialization;
using AwesomeGame2.Shared.ViewModels;

var gameManager = new GameManager();
GameSave save = gameManager.StartNewGame(new StartNewGameCommand("ConsoleHero"));
var toast = new ToastViewModel("New game started.");

Console.WriteLine("Awesome Game 2 - Stage 2 Vertical Slice");
Console.WriteLine("Type 'help' for commands, 'save' for JSON, 'load' to round-trip, 'quit' to exit.");

while (true)
{
    var screen = gameManager.BuildCurrentScreen(save, toast);
    RenderScreen(screen);

    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        toast = new ToastViewModel("Input required.");
        continue;
    }

    var normalized = input.Trim();
    if (normalized.Equals("quit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (normalized.Equals("help", StringComparison.OrdinalIgnoreCase))
    {
        toast = new ToastViewModel("Use menu command ids (forest/search/attack/etc) or special commands help/save/load/quit.");
        continue;
    }

    if (normalized.Equals("save", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine(GameSaveSerializer.SaveToJson(save));
        toast = new ToastViewModel("Displayed save JSON.");
        continue;
    }

    if (normalized.Equals("load", StringComparison.OrdinalIgnoreCase))
    {
        var json = GameSaveSerializer.SaveToJson(save);
        save = GameSaveSerializer.LoadFromJson(json);
        toast = new ToastViewModel("Save/load round trip complete.");
        continue;
    }

    try
    {
        var command = ResolveCommand(screen, normalized);
        if (command == null)
        {
            toast = new ToastViewModel("Unknown command for current screen.");
            continue;
        }

        toast = gameManager.Execute(save, command);
    }
    catch (Exception ex)
    {
        toast = new ToastViewModel("Error: " + ex.Message);
    }
}

static GameCommand? ResolveCommand(ScreenViewModel screen, string input)
{
    if (input == "forest") return new EnterForestCommand();
    if (input == "search") return new SearchForestCommand();
    if (input == "attack") return new AttackCommand();
    if (input == "flee") return new FleeCommand();
    if (input == "weapon_shop") return new VisitShopCommand("weapon_shop");
    if (input == "armor_shop") return new VisitShopCommand("armor_shop");
    if (input == "character") return new ShowCharacterCommand();
    if (input == "end_day") return new EndDayCommand();
    if (input == "return") return new ReturnToTownCommand();

    if (input.StartsWith("buy ", StringComparison.Ordinal))
    {
        return new BuyItemCommand(input.Substring(4).Trim());
    }

    if (input.StartsWith("equip ", StringComparison.Ordinal))
    {
        return new EquipItemCommand(input.Substring(6).Trim());
    }

    if (screen is ShopScreenViewModel && input == "buy")
    {
        return new BuyItemCommand(((ShopScreenViewModel)screen).Items.First().ItemId);
    }

    return null;
}

static void RenderScreen(ScreenViewModel screen)
{
    Console.WriteLine();
    Console.WriteLine("=== " + screen.Title + " ===");
    Console.WriteLine(screen.Toast.Message);
    Console.WriteLine("Player: " + screen.PlayerStatus.Name + " Lv " + screen.PlayerStatus.Level + " HP " + screen.PlayerStatus.Health + "/" + screen.PlayerStatus.MaxHealth);
    Console.WriteLine("Gold: " + screen.PlayerStatus.Gold + " XP: " + screen.PlayerStatus.Experience + " Atk: " + screen.PlayerStatus.Attack + " Def: " + screen.PlayerStatus.Defense);
    Console.WriteLine("Day " + screen.PlayerStatus.DayNumber + " | Actions " + screen.PlayerStatus.RemainingDailyActions + " | Location " + screen.PlayerStatus.CurrentLocation);
    Console.WriteLine("Equipped: " + screen.PlayerStatus.EquippedWeapon + " / " + screen.PlayerStatus.EquippedArmor);

    if (screen is BattleScreenViewModel battle)
    {
        Console.WriteLine("Enemy: " + battle.EnemyName + " HP " + battle.EnemyHealth + "/" + battle.EnemyMaxHealth);
        Console.WriteLine("Battle: " + battle.BattleMessage);
    }

    if (screen is ShopScreenViewModel shop)
    {
        Console.WriteLine(shop.ShopName + " inventory:");
        foreach (var item in shop.Items)
        {
            Console.WriteLine(" - " + item.ItemId + " | " + item.Name + " | " + item.Price + "g | atk+" + item.AttackBonus + " def+" + item.DefenseBonus + (item.Owned ? " | owned" : string.Empty) + (item.Equipped ? " | equipped" : string.Empty));
        }
    }

    if (screen is CharacterScreenViewModel character)
    {
        Console.WriteLine("Inventory:");
        foreach (var line in character.InventoryLines)
        {
            Console.WriteLine(" - " + line);
        }
    }

    Console.WriteLine("Options:");
    foreach (var option in screen.MenuOptions)
    {
        Console.WriteLine(" - " + option.CommandId + " => " + option.Label);
    }
}
