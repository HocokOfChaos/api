using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class ShopExchangeCurrencyLogEntry : IPlayerLogEntry
    {
        public IPlayerLogEntry.Type LogType => IPlayerLogEntry.Type.ShopExchangeCurrency;
        public int Crystals { get; set; }

        // Required for auto mapper, dapper, etc
        public ShopExchangeCurrencyLogEntry() : this(0)
        {
        }
        public ShopExchangeCurrencyLogEntry(int crystals)
        {
            Crystals = crystals;
        }
    }
}
