using AutoMapper;
using RoshdefAPI.AutoMapper.Extensions;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Shared.Models;

namespace RoshdefAPI.AutoMapper.Converter
{
    public class ShopItemDropPlayerItemConverter : ITypeConverter<ShopItemDrop, PlayerItem>
    {
        public PlayerItem Convert(ShopItemDrop source, PlayerItem destination, ResolutionContext context)
        {
            destination = new PlayerItem(source.ID, context.GetPlayerID());
            return destination;
        }
    }
}
