using AutoMapper;
using RoshdefAPI.AutoMapper.Extensions;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Shared.Models;

namespace RoshdefAPI.AutoMapper.Converter
{
    public class ShopItemPlayerItemConverter : ITypeConverter<ShopItem, PlayerItem>
    {
        public PlayerItem Convert(ShopItem source, PlayerItem destination, ResolutionContext context)
        {
            destination = new PlayerItem(source.ID, context.GetPlayerID());
            return destination;
        }
    }
}
