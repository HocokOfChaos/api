using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class AdminPanelDeleteItemLogEntry : IAdminPanelLogEntry
    {
        public IPlayerLogEntry.Type LogType => IPlayerLogEntry.Type.AdminPanelDeleteItem;
        public string AdminLogin { get; set; }
        public ulong ItemID { get; set; }

        // Required for auto mapper, dapper, etc
        public AdminPanelDeleteItemLogEntry() : this("bugged user", 0)
        {
        }
        public AdminPanelDeleteItemLogEntry(string adminLogin, ulong itemID)
        {
            AdminLogin = adminLogin;
            ItemID = itemID;
        }
    }
}
