using RoshdefAPI.Data.Attributes;
using RoshdefAPI.Data.Comparers;
using RoshdefAPI.Data.Constants;
using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class Match : IDataObject
    {
        public ulong ID { get; set; }
        public ulong DotaMatchID { get; set; } = MatchConstants.InvalidDOTAMatchID;
        [NotExistsInDatabaseTable]
        public HashSet<MatchPlayer> Players = new(new MatchPlayerComparer());
        public uint Difficulty { get; set; } = MatchConstants.ExplorationDifficulty;
        public uint Winner { get; set; } = MatchConstants.UnknownWinner;
        public uint Duration { get; set; } = 0;
        public DateTime? EndDateTime { get; set; } = null;

        // Required for auto mapper, dapper, etc
        public Match() : this(MatchConstants.InvalidDOTAMatchID)
        {

        }

        public Match(ulong dotaMatchID)
        {
            DotaMatchID = dotaMatchID;
        }

        public bool IsFinished()
        {
            return EndDateTime != null;
        }
    }
}
