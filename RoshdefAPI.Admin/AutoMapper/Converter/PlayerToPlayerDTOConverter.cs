using AutoMapper;
using RoshdefAPI.Admin.AutoMapper.Extensions;
using RoshdefAPI.Admin.Models.DTO;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Shared.Models;
using RoshdefAPI.Shared.Services.Core;

namespace RoshdefAPI.AutoMapper.Converter
{
    public class PlayerToPlayerDTOConverter : ITypeConverter<Player, PlayerDTO>
    {
        private readonly IDOTALocalizationService _localizationService;
        private readonly IShopItemsService _shopItemsService;
        private readonly IMapper _mapper;
        private readonly ILogger<PlayerToPlayerDTOConverter> _logger;

        public PlayerToPlayerDTOConverter(ILogger<PlayerToPlayerDTOConverter> logger, IDOTALocalizationService localizationService, IShopItemsService shopItemsService, IMapper mapper)
        {
            _localizationService = localizationService;
            _shopItemsService = shopItemsService;
            _mapper = mapper;
            _logger = logger;
        }

        public PlayerDTO Convert(Player source, PlayerDTO destination, ResolutionContext context)
        {
            destination = new PlayerDTO
            {
                SteamID = source.SteamID.ToString(),
            };
            destination.Balance.Crystals = source.Crystals;
            destination.Balance.SoulStones = source.SoulStones;
            destination.Balance.Coins = source.Coins;
            foreach (var playerItem in source.Items)
            {
                ShopItem? shopItem = _shopItemsService.GetItemByID(playerItem.ItemID);
                if (shopItem == null)
                {
                    _logger.LogError("Player have item with {id} that doesn't exists in shop_items.kv", playerItem.ItemID);
                    continue;
                }
                destination.Items.Add(_mapper.Map<PlayerItemDTO>(playerItem, opt =>
                {
                    opt.SetLocalizedName(_localizationService.GetLocalizedString(shopItem.Name));
                }));
            }
            return destination;
        }
    }
}
