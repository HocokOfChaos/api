using AutoMapper;
using RoshdefAPI.AutoMapper.Extensions;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Models.DTO;

namespace RoshdefAPI.AutoMapper.Converter
{
    public class LeaderboardPlayerToLeaderboardPlayerDTOConverter : ITypeConverter<LeaderboardPlayer, LeaderboardPlayerDTO>
    {
        public LeaderboardPlayerDTO Convert(LeaderboardPlayer source, LeaderboardPlayerDTO destination, ResolutionContext context)
        {
            destination = new LeaderboardPlayerDTO(source.SteamID.ToString(), context.GetPlace(), context.GetPoints());
            return destination;
        }
    }
}
