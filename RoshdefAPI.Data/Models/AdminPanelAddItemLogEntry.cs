using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class AdminPanelAddItemLogEntry : IAdminPanelLogEntry
    {
        public IPlayerLogEntry.Type LogType => IPlayerLogEntry.Type.AdminPanelAddItem;
        public string AdminLogin { get; set; }
        public ulong ItemID { get; set; }
        public uint Count { get; set; } = 1;
        public TimeSpan? Duration { get; set; } = null;

        // Required for auto mapper, dapper, etc
        public AdminPanelAddItemLogEntry() : this("bugged user", 0, 0, null)
        {
        }
        public AdminPanelAddItemLogEntry(string adminLogin, ulong itemID, uint count, TimeSpan? duration)
        {
            AdminLogin = adminLogin;
            ItemID = itemID;
            Count = count;
            Duration = duration;
        }
    }
}
