using AutoMapper;
using RoshdefAPI.Data.Models;

namespace RoshdefAPI.AutoMapper.Extensions
{
    public static class ShopItemConverterExtension
    {
        static readonly string PlayerIDKey = nameof(PlayerItem.PlayerID);

        public static ulong GetPlayerID(this ResolutionContext context)
        {
            if (context.Items.TryGetValue(PlayerIDKey, out var data))
            {
                return (ulong)data;
            }

            throw new InvalidOperationException($"{nameof(PlayerItem.PlayerID)} not set.");
        }

        public static IMappingOperationOptions SetPlayerID(this IMappingOperationOptions options, ulong playerID)
        {
            options.Items[PlayerIDKey] = playerID;
            // return options to support fluent chaining.
            return options;
        }
    }
}
