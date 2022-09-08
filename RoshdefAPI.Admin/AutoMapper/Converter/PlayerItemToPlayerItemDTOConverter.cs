using AutoMapper;
using RoshdefAPI.Admin.AutoMapper.Extensions;
using RoshdefAPI.Admin.Models.DTO;
using RoshdefAPI.Data.Models;

namespace RoshdefAPI.AutoMapper.Converter
{
    public class PlayerItemToPlayerItemDTOConverter : ITypeConverter<PlayerItem, PlayerItemDTO>
    {
        public PlayerItemDTO Convert(PlayerItem source, PlayerItemDTO destination, ResolutionContext context)
        {
            destination = new PlayerItemDTO
            {
                ExpireDate = source.ExpireDate.HasValue ? source.ExpireDate.Value.ToUniversalTime() : source.ExpireDate,
                ItemID = source.ItemID.ToString(),
                Count = source.Count,
                Name = context.GetLocalizedName()
            };
            return destination;
        }
    }
}
