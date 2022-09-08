using AutoMapper;
using RoshdefAPI.Admin.Models.DTO;
using RoshdefAPI.AutoMapper.Converter;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Shared.Models;

namespace RoshdefAPI.Admin.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PlayerItem, PlayerItemDTO>()
                .ConvertUsing<PlayerItemToPlayerItemDTOConverter>();
            CreateMap<Player, PlayerDTO>()
                .ConvertUsing<PlayerToPlayerDTOConverter>();
            CreateMap<ShopItem, ShopItemDescriptionDTO>()
                .ConvertUsing<ShopItemToShopItemDescriptionDTOConverter>();
            CreateMap<Player, PlayerBalanceDTO>();
            CreateMap<PlayerLog, PlayerLogDTO>()
                .ConvertUsing<PlayerLogToPlayerLogDTOConverter>();
        }
    }
}
