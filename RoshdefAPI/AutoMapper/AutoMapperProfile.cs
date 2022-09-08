using AutoMapper;
using RoshdefAPI.AutoMapper.Converter;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Models.DTO;
using RoshdefAPI.Shared.Models;

namespace RoshdefAPI.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Player, PlayerBalanceDTO>();
            CreateMap<PlayerItem, PlayerItemDTO>();
            CreateMap<Player, PlayerDTO>()
            .ForMember(t => t.Balance, opt =>
            {
                opt.MapFrom(s => new PlayerBalanceDTO { Coins = s.Coins, Crystals = s.Crystals, SoulStones = s.SoulStones });
            });
            CreateMap<DailyRewardItem, PlayerItem>()
            .ConvertUsing<DailyRewardItemPlayerItemConverter>();
            CreateMap<ShopItem, PlayerItem>()
            .ConvertUsing<ShopItemPlayerItemConverter>();
            CreateMap<ShopItemDrop, PlayerItem>()
            .ConvertUsing<ShopItemDropPlayerItemConverter>();
            CreateMap<LeaderboardPlayer, LeaderboardPlayerDTO>()
            .ConvertUsing<LeaderboardPlayerToLeaderboardPlayerDTOConverter>();
            CreateMap<Player, LeaderboardPlayer>()
            .ForMember(t => t.MonthPoints, opt =>
            {
                opt.Ignore();
            })
            .ForMember(t => t.WeekPoints, opt =>
            {
                opt.Ignore();
            })
            .ForMember(t => t.DayPoints, opt =>
            {
                opt.Ignore();
            });
            CreateMap<MatchPlayer, PlayerBalanceDTO>();
        }
    }
}
