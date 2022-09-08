using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class DailyRewardItem : IDataObject
    {
        public ulong ID { get; set; }
        public ulong ItemID { get; }
        public uint Count { get; } = 1;
        public int? Duration { get; } = null;
        public ulong DailyRewardID { get; }
    }
}
