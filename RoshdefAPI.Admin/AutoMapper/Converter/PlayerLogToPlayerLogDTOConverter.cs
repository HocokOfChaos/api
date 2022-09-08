using AutoMapper;
using RoshdefAPI.Admin.Models.DTO;
using RoshdefAPI.Admin.Services.Core;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Shared.Services.Core;
using System.Text.Json;
using System.Text.Json.Nodes;
using PlayerLogType = RoshdefAPI.Data.Models.Core.IPlayerLogEntry.Type;

namespace RoshdefAPI.AutoMapper.Converter
{
    public class PlayerLogToPlayerLogDTOConverter : ITypeConverter<PlayerLog, PlayerLogDTO>
    {
        private readonly IJsonStringLocalizer _localizer;
        private readonly IDOTALocalizationService _localizationService;
        private readonly IShopItemsService _shopItemsService;
        public PlayerLogToPlayerLogDTOConverter(IJsonStringLocalizer localizer, IDOTALocalizationService localizationService, IShopItemsService shopItemsService)
        {
            _localizer = localizer;
            _localizationService = localizationService;
            _shopItemsService = shopItemsService;
        }

        public PlayerLogDTO Convert(PlayerLog source, PlayerLogDTO destination, ResolutionContext context)
        {
            var result = new PlayerLogDTO
            {
                Date = source.Date,
                Content = FormatPlayerLog(source.Content)
            };
            return result;
        }

        public string FormatPlayerLog(string content)
        {
            var result = $"Unknown log entry {content}";
            var parsedJson = JsonNode.Parse(content);
            if (parsedJson == null)
            {
                return result;
            }
            JsonObject obj = parsedJson.AsObject();
            var logType = (PlayerLogType)(int)obj[nameof(IPlayerLogEntry.LogType)];
            switch (logType)
            {
                case PlayerLogType.ShopBuyItem:
                    var buyItemLog = JsonSerializer.Deserialize<ShopBuyItemLogEntry>(content);
                    if (buyItemLog is not null)
                    {
                        result = string.Format(
                            _localizer.GetLocalizedString("PlayersLogs.ShopBuyItemLogEntry"),
                            GetLocalizedShopItemName(buyItemLog.ItemID),
                            buyItemLog.Crystals,
                            buyItemLog.SoulStones,
                            buyItemLog.Coins
                        );
                    }
                    break;
                case PlayerLogType.ShopExchangeCurrency:
                    var exchangeCurrencyLog = JsonSerializer.Deserialize<ShopExchangeCurrencyLogEntry>(content);
                    if (exchangeCurrencyLog is not null)
                    {
                        result = string.Format(
                            _localizer.GetLocalizedString("PlayersLogs.ShopExchangeCurrencyLogEntry"),
                            exchangeCurrencyLog.Crystals
                        );
                    }
                    break;
                case PlayerLogType.ShopOpenChest:
                    var openChestLog = JsonSerializer.Deserialize<ShopOpenChestLogEntry>(content);
                    if (openChestLog is not null)
                    {
                        result = string.Format(
                            _localizer.GetLocalizedString("PlayersLogs.ShopOpenChestLogEntry"),
                            GetLocalizedShopItemName(openChestLog.ItemID),
                            openChestLog.Count
                        );
                    }
                    break;
                case PlayerLogType.AdminPanelChangeCurrency:
                    var changeCurrenctLog = JsonSerializer.Deserialize<AdminPanelChangeCurrencyLogEntry>(content);
                    if (changeCurrenctLog is not null)
                    {
                        result = string.Format(
                            _localizer.GetLocalizedString("PlayersLogs.AdminPanelChangeCurrencyLogEntry"),
                            changeCurrenctLog.AdminLogin,
                            changeCurrenctLog.Crystals,
                            changeCurrenctLog.SoulStones,
                            changeCurrenctLog.Coins,
                            changeCurrenctLog.CrystalsNew,
                            changeCurrenctLog.SoulStonesNew,
                            changeCurrenctLog.CoinsNew
                        );
                    }
                    break;
                case PlayerLogType.AdminPanelDeleteItem:
                    var deleteItemLog = JsonSerializer.Deserialize<AdminPanelDeleteItemLogEntry>(content);
                    if (deleteItemLog is not null)
                    {
                        result = string.Format(
                            _localizer.GetLocalizedString("PlayersLogs.AdminPanelDeleteItemLogEntry"),
                            deleteItemLog.AdminLogin,
                            GetLocalizedShopItemName(deleteItemLog.ItemID)
                        );
                    }
                    break;
                case PlayerLogType.AdminPanelAddItem:
                    var addItemLog = JsonSerializer.Deserialize<AdminPanelAddItemLogEntry>(content);
                    if (addItemLog is not null)
                    {
                        result = string.Format(
                            _localizer.GetLocalizedString("PlayersLogs.AdminPanelAddItemLogEntry"),
                            addItemLog.AdminLogin,
                            GetLocalizedShopItemName(addItemLog.ItemID),
                            addItemLog.Count,
                            GetLocalizedTimeSpan(addItemLog.Duration)
                        );
                    }
                    break;
                case PlayerLogType.AdminPanelUpdateItem:
                    var updateItemLog = JsonSerializer.Deserialize<AdminPanelUpdateItemLogEntry>(content);
                    if (updateItemLog is not null)
                    {
                        result = string.Format(
                            _localizer.GetLocalizedString("PlayersLogs.AdminPanelUpdateItemLogEntry"),
                            updateItemLog.AdminLogin,
                            GetLocalizedShopItemName(updateItemLog.ItemID),
                            updateItemLog.Count,
                            updateItemLog.CountNew,
                            GetLocalizedDateTime(updateItemLog.ExpireDate),
                            GetLocalizedDateTime(updateItemLog.ExpireDateNew)
                        );
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public string GetLocalizedShopItemName(ulong itemID)
        {
            var shopItem = _shopItemsService.GetItemByID(itemID);
            if (shopItem != null)
            {
                return _localizationService.GetLocalizedString(shopItem.Name);
            }
            return $"Unknown item ({itemID})";
        }

        public string GetLocalizedTimeSpan(TimeSpan? ts)
        {
            if(ts.HasValue)
            {
                return ts.Value.ToString(_localizer.GetLocalizedString("TimeSpan.Format"));
            }
            return _localizer.GetLocalizedString("TimeSpan.Null");
        }

        public string GetLocalizedDateTime(DateTime? dt)
        {
            if (dt.HasValue)
            {
                return dt.Value.ToString(_localizer.GetLocalizedString("DateTime.Format"));
            }
            return _localizer.GetLocalizedString("DateTime.Null");
        }
    }
}
