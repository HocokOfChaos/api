using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class PlayerItem : IDataObject
    {
        public ulong ID { get; set; }
        public ulong ItemID { get; }
        public uint Count { get; set; } = 1;
        public bool? IsDressed { get; set; } = null;
        public DateTime? ExpireDate { get; set; } = null;
        public ulong PlayerID { get; private set;  }

        // Required for auto mapper, dapper, etc
        public PlayerItem()
        {

        }

        public PlayerItem(ulong itemID, ulong playerID)
        {
            ItemID = itemID;
            PlayerID = playerID;
        }

        public bool IsExpired(DateTime now)
        {
            if (ExpireDate != null && now.CompareTo(ExpireDate) > 0)
            {
                return true;
            }
            return false;
        }
    }
}
