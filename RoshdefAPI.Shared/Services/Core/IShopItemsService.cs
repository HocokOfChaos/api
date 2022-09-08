using RoshdefAPI.Shared.Models;

namespace RoshdefAPI.Shared.Services.Core
{
    public interface IShopItemsService
    {
        public IEnumerable<ShopItem> GetAllItems();
        public ShopItem? GetItemByID(ulong id);
        public void LoadKeyValues();
        public bool IsKeyValuesLoaded();
    }
}
