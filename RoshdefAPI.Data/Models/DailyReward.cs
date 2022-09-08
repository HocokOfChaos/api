using RoshdefAPI.Data.Attributes;
using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class DailyReward : IDataObject
    {
        public ulong ID { get; set; }
        public uint DayNumber { get; } = 1;
        [NotExistsInDatabaseTable]
        public List<DailyRewardItem> Items { get; } = new List<DailyRewardItem>();

        public int Crystals { get; } = 0;
        public int Coins { get; } = 0;
        public int SoulStones { get; } = 0;
    }
}
