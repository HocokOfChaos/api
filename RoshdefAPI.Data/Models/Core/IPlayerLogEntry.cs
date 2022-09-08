namespace RoshdefAPI.Data.Models.Core
{
    public interface IPlayerLogEntry
    {
        // Don't change enum values once established
        public enum Type
        {
            Invalid = -1,
            ShopBuyItem = 1,
            ShopExchangeCurrency = 2,
            ShopOpenChest = 3,
            AdminPanelChangeCurrency = 4,
            AdminPanelDeleteItem = 5,
            AdminPanelAddItem = 6,
            AdminPanelUpdateItem = 7
        }

        public abstract @Type LogType { get; }
    }
}
