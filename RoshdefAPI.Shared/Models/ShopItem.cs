using System.Collections.ObjectModel;

namespace RoshdefAPI.Shared.Models
{
    public enum ShopItemType
    {
        Effect = 0,
        Chests = 1,
        Unused1 = 2,
        Unused2 = 3,
        Pet = 4,
        Consumable = 5,
        Hero = 6,
        Subscription = 7,
        Invalid = 8
    }
    public enum ShopItemPriceType
    {
        Coins = 0,
        Crystals = 1,
        SoulStones = 2,
        Invalid = 3
    }

    public class ShopItem
    {
        public ulong ID { get; private set; }
        public string Name { get; private set; } = "";
        public ShopItemType Type { get; private set; } = ShopItemType.Invalid;
        public bool Buyable { get; private set; } = false;
        public int Price { get; private set; } = 0;
        public ShopItemPriceType PriceType { get; private set; } = ShopItemPriceType.Invalid;
        public ReadOnlyCollection<ShopItemDrop> DropItems { get; private set; }
        public bool FollowPseudoRandom { get; private set; }  = true;
        public int DropCount { get; private set; }  = 0;
        public int? Duration { get; private set; } = null;
        public Dictionary<ulong, ShopItem> AdditionalItems {get; private set;} = new Dictionary<ulong, ShopItem>();
        public ShopItem(ulong id, string itemName, ShopItemType type, bool buyable, int price, ShopItemPriceType priceType, bool followPseudoRandom, int dropCount, ReadOnlyCollection<ShopItemDrop> dropList, int? duration = null)
        {
            ID = id;
            Name = itemName;
            Type = type;
            Buyable = buyable;
            Price = price;
            PriceType = priceType;
            FollowPseudoRandom = followPseudoRandom;
            DropCount = dropCount;
            DropItems = dropList ?? new List<ShopItemDrop>().AsReadOnly();
            Duration = duration;
        }

        public bool IsStackable()
        {
            return Type == ShopItemType.Chests || Type == ShopItemType.Hero || Type == ShopItemType.Consumable;
        }
    }
}
