using RoshdefAPI.Data.Constants;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Data.Extensions;

namespace RoshdefAPI.Data.Models
{
    public class LeaderboardPlayer : IDataObject
    {
        public ulong ID { get; set; }
        public ulong SteamID { get; private set; }
        public ulong MonthPoints { get; private set; } = 0;
        public ulong WeekPoints { get; private set; } = 0;
        public ulong DayPoints { get; private set; } = 0;

        // Required for auto mapper, dapper, etc
        public LeaderboardPlayer() : this(PlayerConstants.InvalidSteamID)
        {

        }

        public LeaderboardPlayer(ulong steamID)
        {
            SteamID = steamID;
        }

        public void AddPoints(ulong points)
        {
            if (MonthPoints.IsAdditionWillCauseOverflow(points))
            {
                MonthPoints = ulong.MaxValue;
            }
            else
            {
                MonthPoints += points;
            }
            if (WeekPoints.IsAdditionWillCauseOverflow(points))
            {
                WeekPoints = ulong.MaxValue;
            }
            else
            {
                WeekPoints += points;
            }
            if (DayPoints.IsAdditionWillCauseOverflow(points))
            {
                DayPoints = ulong.MaxValue;
            }
            else
            {
                DayPoints += points;
            }
        }
    }
}
