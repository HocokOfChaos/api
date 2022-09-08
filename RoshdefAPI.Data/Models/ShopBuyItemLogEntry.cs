using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class ShopBuyItemLogEntry : IPlayerLogEntry
    {
        public IPlayerLogEntry.Type LogType => IPlayerLogEntry.Type.ShopBuyItem;
        public ulong ItemID { get; set; }
        public int Crystals { get; set; }
        public int SoulStones { get; set; }
        public int Coins { get; set; }

        // Required for auto mapper, dapper, etc
        public ShopBuyItemLogEntry() : this(0, 0, 0, 0)
        {
        }

        public ShopBuyItemLogEntry(ulong itemID, int crystals, int soulStones, int coins)
        {
            ItemID = itemID;
            Crystals = crystals;
            SoulStones = soulStones;
            Coins = coins;
        }

        public void SetCoins(int coins)
        {
            Coins = coins;
        }

        public void SetCrystals(int crystals)
        {
            Crystals = crystals;
        }

        public void SetSoulStones(int soulStones)
        {
            SoulStones = soulStones;
        }
    }
}
