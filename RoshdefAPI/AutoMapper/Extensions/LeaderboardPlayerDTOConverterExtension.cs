using AutoMapper;
using RoshdefAPI.Models.DTO;

namespace RoshdefAPI.AutoMapper.Extensions
{
    public static class LeaderboardPlayerDTOConverterExtension
    {
        static readonly string PointsKey = nameof(LeaderboardPlayerDTO.Points);
        static readonly string PlaceKey = nameof(LeaderboardPlayerDTO.Place);

        public static ulong GetPoints(this ResolutionContext context)
        {
            if (context.Items.TryGetValue(PointsKey, out var data))
            {
                return (ulong)data;
            }

            throw new InvalidOperationException($"{nameof(LeaderboardPlayerDTO.Points)} not set.");
        }

        public static IMappingOperationOptions SetPoints(this IMappingOperationOptions options, ulong points)
        {
            options.Items[PointsKey] = points;
            // return options to support fluent chaining.
            return options;
        }

        public static uint GetPlace(this ResolutionContext context)
        {
            if (context.Items.TryGetValue(PlaceKey, out var data))
            {
                return (uint)data;
            }

            throw new InvalidOperationException($"{nameof(LeaderboardPlayerDTO.Place)} not set.");
        }

        public static IMappingOperationOptions SetPlace(this IMappingOperationOptions options, uint place)
        {
            options.Items[PlaceKey] = place;
            // return options to support fluent chaining.
            return options;
        }
    }
}
