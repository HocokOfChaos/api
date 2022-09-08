using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class AdminPanelUpdateItemLogEntry : IAdminPanelLogEntry
    {
        public IPlayerLogEntry.Type LogType => IPlayerLogEntry.Type.AdminPanelUpdateItem;
        public string AdminLogin { get; set; }
        public ulong ItemID { get; set; }
        public uint Count { get; set; }
        public uint CountNew { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? ExpireDateNew { get; set; }

        // Required for auto mapper, dapper, etc
        public AdminPanelUpdateItemLogEntry() : this("bugged user", 0, 0, 0, null, null)
        {
        }
        public AdminPanelUpdateItemLogEntry(string adminLogin, ulong itemID, uint count, uint countNew, DateTime? expireDate, DateTime? expireDateNew)
        {
            Count = count;
            CountNew = countNew;
            ExpireDate = expireDate;
            ExpireDateNew = expireDateNew;
            AdminLogin = adminLogin;
            ItemID = itemID;
        }
    }
}
