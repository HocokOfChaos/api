using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Attributes.Routing;
using RoshdefAPI.AutoMapper.Extensions;
using RoshdefAPI.Controllers.Core;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Request;
using RoshdefAPI.Shared.Models;
using RoshdefAPI.Shared.Services.Core;
using System.Net.Mime;
using static RoshdefAPI.Data.Models.Config;

namespace RoshdefAPI.Controllers
{
    [Produces(MediaTypeNames.Application.Json), ApiController, APIRoute("[controller]")]
    public class ShopController : APIController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlayersRepositoryBase _playerRepository;
        private readonly PlayersItemsRepositoryBase _playerItemsRepository;
        private readonly IShopItemsService _shopItemsService;
        private readonly IMapper _mapper;
        private readonly ConfigRepositoryBase _configRepository;
        private readonly PlayersLogsRepositoryBase _playersLogsRepository;

        public ShopController(
            ILogger<ShopController> logger, 
            IUnitOfWork uow, 
            PlayersRepositoryBase playerRepository, 
            PlayersItemsRepositoryBase playerItemsRepository, 
            IShopItemsService shopItemsService, 
            IMapper mapper, 
            ConfigRepositoryBase configRepository, 
            PlayersLogsRepositoryBase playersLogsRepository
        ) : base(logger)
        {
            _unitOfWork = uow;
            _playerRepository = playerRepository;
            _playerItemsRepository = playerItemsRepository;
            _shopItemsService = shopItemsService;
            _mapper = mapper;
            _configRepository = configRepository;
            _playersLogsRepository = playersLogsRepository;
        }

        [HttpPost, Route("BuyItem")]
        public async Task<ShopBuyItemResponse> BuyItem(ShopBuyItemRequest request)
        {
            var response = new ShopBuyItemResponse { Success = false };

            var steamID = request.Data.SteamID;
            var itemID = request.Data.ItemID;
            var shopItem = _shopItemsService.GetItemByID(itemID);
            if (shopItem == null)
            {
                return Error(ref response, $"Attempt to buy item with unknown id ({nameof(request.Data.ItemID)} = {itemID}).");
            }
            if (shopItem.Buyable == false)
            {
                return Error(ref response, $"Attempt to buy not buyable item ({nameof(request.Data.ItemID)} = {itemID}, {nameof(shopItem.Buyable)} = false).");
            }
            var player = await _playerRepository.FindBySteamID(steamID, true);
            if (player == null)
            {
                return Error(ref response, $"Attempt to buy item for non registered player ({nameof(request.Data.SteamID)} = {steamID}).");
            }
            var itemPrice = shopItem.Price;
            var itemPriceType = shopItem.PriceType;
            var isPlayerCanAffordItem = false;
            var logEntry = new ShopBuyItemLogEntry(itemID, 0, 0, 0);
            switch (itemPriceType)
            {
                case ShopItemPriceType.Crystals:
                    if (player.Crystals >= itemPrice)
                    {
                        player.AddCrystals(-itemPrice);
                        logEntry.SetCrystals(itemPrice);
                        isPlayerCanAffordItem = true;
                    }
                    break;
                case ShopItemPriceType.SoulStones:
                    if (player.SoulStones >= itemPrice)
                    {
                        player.AddSoulStones(-itemPrice);
                        logEntry.SetSoulStones(itemPrice);
                        isPlayerCanAffordItem = true;
                    }
                    break;
                case ShopItemPriceType.Coins:
                    if (player.Coins >= itemPrice)
                    {
                        player.AddCoins(-itemPrice);
                        logEntry.SetCoins(itemPrice);
                        isPlayerCanAffordItem = true;
                    }
                    break;
                default:
                case ShopItemPriceType.Invalid:
                    isPlayerCanAffordItem = false;
                    break;
            }
            if (!isPlayerCanAffordItem)
            {
                return Error(ref response, $"Player can't afford specified item ({nameof(request.Data.ItemID)} = {itemID}). Not enough balance.");
            }
            var newItem = _mapper.Map<PlayerItem>(shopItem, options => options.SetPlayerID(player.ID));
            TimeSpan? newItemDuration = shopItem.Duration.HasValue ? TimeSpan.FromSeconds(shopItem.Duration.Value) : null;
            (var isAdded, var addedItem) = player.AddItem(newItem, newItemDuration);
            if (isAdded)
            {
                await _playerItemsRepository.Insert(addedItem);
            }
            else
            {
                await _playerItemsRepository.Update(addedItem);
            }
            await _playersLogsRepository.Log(steamID, logEntry);
            await AddAdditionalItemsToBoughtItemIfRequired(player, newItem, newItemDuration);
            await _playerRepository.Update(player);
            var playerDataDTO = _mapper.Map<Player, PlayerDTO>(player);
            response.Success = true;
            response.Data = new ShopBuyItemResponseData
            {
                Balance = playerDataDTO.Balance,
                Items = playerDataDTO.Items
            };
            _unitOfWork.Commit();
            return response;
        }

        [HttpPost, Route("ExchangeCurrency")]
        public async Task<PlayerExchangeCurrencyResponse> ExchangeCurrency(PlayerExchangeCurrencyRequest request)
        {
            var response = new PlayerExchangeCurrencyResponse { Success = false };
            var playerSteamID = request.Data.SteamID;
            var crystalsForExchange = request.Data.CrystalsForExchange;
            if (crystalsForExchange < 1)
            {
                return Error(ref response, "Crystals must be positive integer greater than 0.");
            }
            var player = await _playerRepository.FindBySteamID(playerSteamID, false);
            if (player == null)
            {
                return Error(ref response, "Attempt to exchange currency for non registered player.");
            }
            if (player.Crystals >= crystalsForExchange)
            {
                var exchangeRateConfig = await _configRepository.GetValue(ConfigType.CrystalsToCoinsExchangeRate);
                if (exchangeRateConfig != null && int.TryParse(exchangeRateConfig.Value, out int exchangeRate))
                {
                    var coinsFromExchange = crystalsForExchange * exchangeRate;
                    player.AddCrystals(-crystalsForExchange);
                    player.AddCoins(coinsFromExchange);
                    await _playerRepository.Update(player);
                    await _playersLogsRepository.Log(playerSteamID, new ShopExchangeCurrencyLogEntry(crystalsForExchange));
                    response.Success = true;
                    response.Data = new PlayerExchangeCurrencyResponseData
                    {
                        Balance = _mapper.Map<Player, PlayerBalanceDTO>(player)
                    };
                    _unitOfWork.Commit();
                    return response;
                }
                else
                {
                    return Error(ref response, "Failed to load exchange rate.");
                }
            }
            else
            {
                return Error(ref response, "Player don't have enough crystals for exchange.");
            }
        }

        [HttpPost, Route("OpenChest")]
        public async Task<ShopOpenTreasureResponse> OpenChest(ShopOpenTreasureRequest request)
        {
            var response = new ShopOpenTreasureResponse { Success = false };
            var playerSteamID = request.Data.SteamID;
            var chestID = request.Data.ItemID;
            var chestsCount = request.Data.Count;
            var shopItem = _shopItemsService.GetItemByID(chestID);
            if (shopItem == null)
            {
                return Error(ref response, $"Attempt to open chest with unknown id ({nameof(request.Data.ItemID)} = {chestID}).");
            }
            if (shopItem.Type != ShopItemType.Chests)
            {
                return Error(ref response, $"Attempt to open chest with invalid type ({nameof(request.Data.ItemID)} = {chestID}; {nameof(shopItem.Type)} = {shopItem.Type}, expected {nameof(ShopItemType.Chests)})");
            }
            if (chestsCount == 0)
            {
                return Error(ref response, "Attempt to open 0 chests.");
            }
            var player = await _playerRepository.FindBySteamID(playerSteamID, true);
            if (player == null)
            {
                return Error(ref response, "Attempt to open chest for non registered player.");
            }
            var chestItem = player.Items.FirstOrDefault(item => item.ItemID.Equals(chestID));
            if (chestItem == null)
            {
                return Error(ref response, "Player doesn't have at least one of specified chest(s).");
            }
            if (chestItem.Count < chestsCount)
            {
                return Error(ref response, "Player doesn't have specified amount of chest(s).");
            }
            if (player.ConsumeItem(chestItem, chestsCount))
            {
                await _playerItemsRepository.Delete(chestItem);
            }
            else
            {
                await _playerItemsRepository.Update(chestItem);
            }
            for (int i = 0; i < chestsCount; i++)
            {
                await AddItemsToPlayerFromChest(player, shopItem);
            }
            await _playersLogsRepository.Log(playerSteamID, new ShopOpenChestLogEntry(chestID, chestsCount));
            var playerDataDTO = _mapper.Map<Player, PlayerDTO>(player);
            response.Success = true;
            response.Data = new ShopOpenTreasureResponseData
            {
                Items = playerDataDTO.Items
            };
            _unitOfWork.Commit();
            return response;
        }

        private async Task AddItemsToPlayerFromChest(Player player, ShopItem chest)
        {
            var itemsForInsertion = new List<PlayerItem>();
            var itemsForUpdating = new List<PlayerItem>();
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(chest);
            if (chest.FollowPseudoRandom)
            {
                // https://jonlabelle.com/snippets/view/csharp/pick-random-elements-based-on-probability
                var itemsInPossibleDropList = chest.DropItems.ToList();
                itemsInPossibleDropList.Sort((a, b) => a.DropChance.CompareTo(b.DropChance));
                var r = new Random();
                for (int i = 0; i < chest.DropCount; i++)
                {
                    double diceRoll = r.NextDouble();
                    double cumulative = 0.0;
                    for (int j = 0; j < itemsInPossibleDropList.Count; j++)
                    {
                        cumulative += itemsInPossibleDropList[j].DropChance / 100;
                        if (diceRoll < cumulative)
                        {
                            var newItem = _mapper.Map<PlayerItem>(itemsInPossibleDropList[j], options => options.SetPlayerID(player.ID));
                            (var isAdded, var addedItem) = player.AddItem(newItem, null);
                            if (isAdded)
                            {
                                itemsForInsertion.Add(addedItem);
                            }
                            else
                            {
                                itemsForUpdating.Add(addedItem);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var droppedItem in chest.DropItems)
                {
                    var newItem = _mapper.Map<PlayerItem>(droppedItem, options => options.SetPlayerID(player.ID));
                    (var isAdded, var addedItem) = player.AddItem(newItem, null);
                    if (isAdded)
                    {
                        itemsForInsertion.Add(addedItem);
                    }
                    else
                    {
                        itemsForUpdating.Add(addedItem);
                    }
                }
            }
            await _playerItemsRepository.BulkUpdate(itemsForUpdating, 25);
            await _playerItemsRepository.BulkInsert(itemsForInsertion, 25);
        }

        private async Task AddAdditionalItemsToBoughtItemIfRequired(Player player, PlayerItem item, TimeSpan? duration = null)
        {
            List<PlayerItem> itemsForInsertion = new();
            List<PlayerItem> itemsForUpdating = new();
            var shopItem = _shopItemsService.GetItemByID(item.ItemID);
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(shopItem);
            if (shopItem.AdditionalItems.Count == 0)
            {
                return;
            }
            foreach (var additionalItem in shopItem.AdditionalItems.Values)
            {
                var newItem = _mapper.Map<PlayerItem>(additionalItem, options => options.SetPlayerID(player.ID));
                (var isAdded, var addedItem) = player.AddItem(newItem, duration);
                if (isAdded)
                {
                    itemsForInsertion.Add(addedItem);
                }
                else
                {
                    itemsForUpdating.Add(addedItem);
                }
            }
            await _playerItemsRepository.BulkUpdate(itemsForUpdating, 25);
            await _playerItemsRepository.BulkInsert(itemsForInsertion, 25);
        }
    }
}
