using AutoMapper;
using RoshdefAPI.Admin.Models.DTO;

namespace RoshdefAPI.Admin.AutoMapper.Extensions
{
    public static class PlayerItemConverterExtension
    {
        static readonly string ItemNameKey = nameof(PlayerItemDTO.Name);

        public static string GetLocalizedName(this ResolutionContext context)
        {
            if (context.Items.TryGetValue(ItemNameKey, out var data))
            {
                return (string)data;
            }

            throw new InvalidOperationException($"{nameof(PlayerItemDTO.Name)} not set.");
        }

        public static IMappingOperationOptions SetLocalizedName(this IMappingOperationOptions options, string itemName)
        {
            options.Items[ItemNameKey] = itemName;
            // return options to support fluent chaining.
            return options;
        }
    }
}
