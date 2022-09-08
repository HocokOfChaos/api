using AutoMapper;
using RoshdefAPI.AutoMapper.Extensions;
using RoshdefAPI.Data.Models;

namespace RoshdefAPI.AutoMapper.Converter
{
    public class DailyRewardItemPlayerItemConverter : ITypeConverter<DailyRewardItem, PlayerItem>
    {
        public PlayerItem Convert(DailyRewardItem source, PlayerItem destination, ResolutionContext context)
        {
            destination = new PlayerItem(source.ItemID, context.GetDailyRewardPlayerID());
            destination.Count = source.Count;
            return destination;
        }
    }
}
