namespace RoshdefAPI.Data.Models.Core
{
    public interface IAdminPanelLogEntry : IPlayerLogEntry
    {
        public abstract string AdminLogin { get; set; }
    }
}
