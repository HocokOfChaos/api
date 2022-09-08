using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class ShopOpenChestLogEntry : IPlayerLogEntry
    {
        public IPlayerLogEntry.Type LogType => IPlayerLogEntry.Type.ShopOpenChest;
        public ulong ItemID { get; set; }
        public uint Count { get; set; }

        // Required for auto mapper, dapper, etc
        public ShopOpenChestLogEntry() : this(0, 0)
        {
        }
        public ShopOpenChestLogEntry(ulong itemID, uint count)
        {
            ItemID = itemID;
            Count = count;
        }
    }
}
