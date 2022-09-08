using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class AdminPanelChangeCurrencyLogEntry : IAdminPanelLogEntry
    {
        public IPlayerLogEntry.Type LogType => IPlayerLogEntry.Type.AdminPanelChangeCurrency;
        public string AdminLogin { get; set; }
        public int Crystals { get; set; } = 0;
        public int Coins { get; set; } = 0;
        public int SoulStones { get; set; } = 0;
        public int CrystalsNew { get; set; } = 0;
        public int CoinsNew { get; set; } = 0;
        public int SoulStonesNew { get; set; } = 0;

        // Required for auto mapper, dapper, etc
        public AdminPanelChangeCurrencyLogEntry() : this("bugged user", 0, 0, 0, 0, 0, 0)
        {
        }
        public AdminPanelChangeCurrencyLogEntry(string adminLogin, int crystals, int coins, int soulStones, int crystalsNew, int coinsNew, int soulStonesNew)
        {
            AdminLogin = adminLogin;
            Crystals = crystals;
            Coins = coins;
            SoulStones = soulStones;
            CrystalsNew = crystalsNew;
            CoinsNew = coinsNew;
            SoulStonesNew = soulStonesNew;
        }
    }
}
