using AutoMapper;
using RoshdefAPI.Admin.Models.DTO;
using RoshdefAPI.Shared.Models;
using RoshdefAPI.Shared.Services.Core;

namespace RoshdefAPI.AutoMapper.Converter
{
    public class ShopItemToShopItemDescriptionDTOConverter : ITypeConverter<ShopItem, ShopItemDescriptionDTO>
    {
        private readonly IDOTALocalizationService _localizationService;

        public ShopItemToShopItemDescriptionDTOConverter(IDOTALocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public ShopItemDescriptionDTO Convert(ShopItem source, ShopItemDescriptionDTO destination, ResolutionContext context)
        {
            return new ShopItemDescriptionDTO
            {
                ItemID = source.ID.ToString(),
                Name = _localizationService.GetLocalizedString(source.Name)
            };
        }
    }
}
