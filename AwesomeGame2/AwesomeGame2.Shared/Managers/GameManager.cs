using System;
using AwesomeGame2.Shared.Commands;
using AwesomeGame2.Shared.Saves;
using AwesomeGame2.Shared.Validation;
using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class GameManager
    {
        private readonly ItemCatalog _itemCatalog;
        private readonly InventoryManager _inventoryManager;
        private readonly ProgressionManager _progressionManager;
        private readonly RandomManager _randomManager;
        private readonly BattleManager _battleManager;
        private readonly WorldManager _worldManager;
        private readonly ShopManager _shopManager;
        private readonly ScreenManager _screenManager;

        public GameManager()
        {
            _itemCatalog = new ItemCatalog();
            _inventoryManager = new InventoryManager(_itemCatalog);
            _progressionManager = new ProgressionManager();
            _randomManager = new RandomManager();
            _battleManager = new BattleManager(_randomManager, _inventoryManager, _progressionManager);
            _worldManager = new WorldManager(_randomManager, _battleManager, _progressionManager);
            _shopManager = new ShopManager(_itemCatalog, _inventoryManager);
            _screenManager = new ScreenManager(new PlayerManager(_inventoryManager), _shopManager, _inventoryManager);
        }

        public GameSave StartNewGame(StartNewGameCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return GameSaveFactory.CreateNew(command.PlayerName);
        }

        public ToastViewModel Execute(GameSave save, GameCommand command)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var message = "Done.";

            switch (command)
            {
                case EnterForestCommand:
                    _worldManager.EnterForest(save);
                    message = "You head into the forest.";
                    break;
                case SearchForestCommand:
                    message = _worldManager.SearchForest(save);
                    break;
                case AttackCommand:
                    message = _battleManager.Attack(save);
                    break;
                case FleeCommand:
                    message = _battleManager.Flee(save);
                    break;
                case VisitShopCommand visitShop:
                    _shopManager.VisitShop(save, visitShop.ShopId);
                    message = "Welcome to the " + visitShop.ShopId + ".";
                    break;
                case BuyItemCommand buyItem:
                    message = _shopManager.BuyItem(save, buyItem.ItemId);
                    break;
                case EquipItemCommand equipItem:
                    _inventoryManager.Equip(save, equipItem.ItemId);
                    message = "Equipped " + equipItem.ItemId + ".";
                    break;
                case EndDayCommand:
                    message = _worldManager.EndDay(save);
                    break;
                case ReturnToTownCommand:
                    _worldManager.ReturnToTown(save);
                    message = "Returned to town.";
                    break;
                case ShowCharacterCommand:
                    save.CurrentLocation = "character";
                    message = "Viewing character.";
                    break;
                case StartNewGameCommand:
                    throw new InvalidOperationException("Use StartNewGame for StartNewGameCommand.");
                default:
                    throw new NotSupportedException("Unsupported command type: " + command.GetType().Name);
            }

            var errors = GameSaveValidator.Validate(save);
            if (errors.Count > 0)
            {
                throw new InvalidOperationException("GameSave invalid after command execution: " + string.Join("; ", errors));
            }

            return new ToastViewModel(message);
        }

        public ScreenViewModel BuildCurrentScreen(GameSave save, ToastViewModel toast)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            return _screenManager.CreateForCurrentLocation(save, toast);
        }
    }
}
