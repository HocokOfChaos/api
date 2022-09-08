using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoshdefAPI.Shared.Models;
using RoshdefAPI.Shared.Models.Configuration;
using RoshdefAPI.Shared.Services.Core;
using ValveKeyValue;

namespace RoshdefAPI.Shared.Services
{
    public class ShopItemsService : IShopItemsService
    {
        private readonly ILogger<ShopItemsService> _logger;
        private readonly Dictionary<ulong, ShopItem> _items;
        private readonly string _pathToKV;
        private bool _isKeyValuesLoaded = false;

        public ShopItemsService(ILogger<ShopItemsService> logger, IOptions<ApplicationSettings> config)
        {
            _logger = logger;
            _pathToKV = config.Value.PathToShopItemsKV;
            if (config.Value.UseRelativePathForKV)
            {
                _pathToKV = Path.Combine(Directory.GetCurrentDirectory(), _pathToKV);
            }
            _items = new Dictionary<ulong, ShopItem>();
        }

        public ShopItem? GetItemByID(ulong id)
        {
            _items.TryGetValue(id, out ShopItem? item);
            return item;
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            return _items.Values;
        }

        private class UnparsedShopItem : IEquatable<UnparsedShopItem>
        {
            public ShopItem Item { get; set; }

            public KVObject? UnparsedAdditionalItems { get; set; }

            public UnparsedShopItem(ShopItem item, KVObject? unparsedSubscriptionItems)
            {
                Item = item;
                UnparsedAdditionalItems = unparsedSubscriptionItems;
            }

            public override bool Equals(object? obj)
            {
                return obj is UnparsedShopItem item && Equals(item);
            }

            public bool Equals(UnparsedShopItem? p)
            {
                if (p?.Item == null)
                {
                    return false;
                }
                return p.Item.ID.Equals(Item.ID);
            }

            public override int GetHashCode()
            {
                return Convert.ToInt32(Item.ID);
            }
        }

        public void LoadKeyValues()
        {
            if (IsKeyValuesLoaded())
            {
                _logger.LogError("Attempt to load key values for shop items more than once.");
                return;
            }
            var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            KVObject kv;
            using (var stream = new FileStream(_pathToKV, FileMode.Open, FileAccess.Read))
            {
                kv = serializer.Deserialize(stream);
            }
            var shopItems = kv.Children.Where(x => x.Name.Equals("Items")).FirstOrDefault();
            if (shopItems == null)
            {
                throw new Exception("Can't find Items key in shop_items.kv.");
            }
            HashSet<UnparsedShopItem> unparsedItems = new();
            foreach (var shopItem in shopItems.Children)
            {
                if (!ulong.TryParse(shopItem.Name.ToString(), out ulong id))
                {
                    _logger.LogError("Error reading item with id {id} (positive integer expected).", id);
                    continue;
                }
                string itemName = "";
                var readedName = shopItem["name"];
                if(readedName is not null)
                {
                    itemName = readedName.ToString();
                }
                var readedType = shopItem["type"];
                if (readedType == null)
                {
                    _logger.LogError("Error reading item with id {id}. Type is missing.", id);
                    continue;
                }
                ShopItemType itemType = ShopItemType.Invalid;
                if (!Enum.TryParse(shopItem["type"].ToString(), out itemType) || itemType == ShopItemType.Invalid)
                {
                    _logger.LogError("Error reading item (id = {id}, type = {itemType}). Invalid or unknown type.", id, itemType);
                    continue;
                }
                var readedBuyable = shopItem["buyable"];
                bool isItemBuyable = false;
                if (readedBuyable != null)
                {
                    if (!int.TryParse(readedBuyable.ToString(), out int tempInt))
                    {
                        _logger.LogError("Error reading item with id {id}. Invalid \"buyable\" field (0 or 1 expected).", id);
                    }
                    if (tempInt < 0 || tempInt > 1)
                    {
                        _logger.LogError("Error reading item with id {id}. Invalid \"buyable\" field (0 or 1 expected).", id);
                        tempInt = 0;
                    }
                    isItemBuyable = (tempInt == 1);
                }
                int itemPrice = 0;
                ShopItemPriceType itemPriceType = ShopItemPriceType.Invalid;
                if (isItemBuyable)
                {
                    var readedPrice = shopItem["price"];
                    if (readedPrice == null)
                    {
                        _logger.LogError("Error reading item with id {id}. Invalid \"price\" field (price array expected for buyable items).", id);
                        continue;
                    }
                    if (readedPrice["crystals"] != null)
                    {
                        if (!int.TryParse(readedPrice["crystals"].ToString(), out itemPrice) || itemPrice < 1)
                        {
                            _logger.LogError("Error reading item with id {id}. Invalid \"price.crystals\" field (positive integer expected).", id);
                        }
                        else
                        {
                            itemPriceType = ShopItemPriceType.Crystals;
                        }
                    }
                    if (readedPrice["coins"] != null)
                    {
                        if (!int.TryParse(readedPrice["coins"].ToString(), out itemPrice) || itemPrice < 1)
                        {
                            _logger.LogError("Error reading item with id {id}. Invalid \"price.coins\" field (positive integer expected).", id);
                        }
                        else
                        {
                            itemPriceType = ShopItemPriceType.Coins;
                        }
                    }
                    if (readedPrice["soul_stones"] != null)
                    {
                        if (!int.TryParse(readedPrice["soul_stones"].ToString(), out itemPrice) || itemPrice < 1)
                        {
                            _logger.LogError("Error reading item with id {id}. Invalid \"price.soul_stones\" field (positive integer expected).", id);
                        }
                        else
                        {
                            itemPriceType = ShopItemPriceType.SoulStones;
                        }
                    }
                    if (itemPriceType == ShopItemPriceType.Invalid)
                    {
                        _logger.LogError("Error reading item with id {id}. Invalid \"price\" field (valid price array expected for buyable items).", id);
                        continue;
                    }
                }
                var readedDropItems = shopItem["drop"];
                var followPseudoRandom = true;
                var dropsCount = 0;
                var dropList = new List<ShopItemDrop>();
                if (readedDropItems != null)
                {
                    var readedFollowPseudoRandom = readedDropItems["follow_pseudo_rnd"];
                    if (readedFollowPseudoRandom != null)
                    {
                        if (!int.TryParse(readedFollowPseudoRandom.ToString(), out int tempInt) || tempInt < 0 || tempInt > 1)
                        {
                            _logger.LogError("Error reading item with id {id}. Invalid \"drop.follow_pseudo_rnd\" field (0 or 1 expected).", id);
                            tempInt = 0;
                        }
                        followPseudoRandom = (tempInt == 1);
                    }
                    if (followPseudoRandom)
                    {
                        var readedDropsCount = readedDropItems["drops_count"];
                        if (readedDropsCount != null)
                        {
                            if (!int.TryParse(readedDropsCount.ToString(), out int tempInt) || tempInt < 0)
                            {
                                _logger.LogError("Error reading item with id {id}. Invalid \"drop.drops_count\" field (positive integer expected).", id);
                                tempInt = 0;
                            }
                            dropsCount = tempInt;
                        }
                        else
                        {
                            _logger.LogError("Error reading item with id {id}. Items with follow_pseudo_rnd field must have drops_count field defined.", id);
                            continue;
                        }
                    }
                    if (dropsCount > 0 || followPseudoRandom == false)
                    {
                        var readedDropItemsArray = shopItem.Children.Where(x => x.Name.Equals("drop")).FirstOrDefault();
                        if (readedDropItemsArray != null)
                        {
                            readedDropItemsArray = readedDropItemsArray.Where(x => x.Name.Equals("items")).FirstOrDefault();
                        }
                        if (readedDropItemsArray != null)
                        {
                            foreach (var shopItemDrop in readedDropItemsArray.Children)
                            {
                                var shopItemDropID = shopItemDrop["id"];
                                uint parsedShopItemDropID;
                                if (shopItemDropID == null)
                                {
                                    _logger.LogError("Error reading item with id {id}. Dropped item {Name} in drop field missing id field.", id, shopItemDrop.Name);
                                    continue;
                                }
                                else
                                {
                                    if (!uint.TryParse(shopItemDropID.ToString(), out uint tempInt) || tempInt < 0)
                                    {
                                        _logger.LogError("Error reading item with id {id}. Dropped item {Name} in drop field has invalid id field (positive integer expected).", id, shopItemDrop.Name);
                                        continue;
                                    }
                                    parsedShopItemDropID = tempInt;
                                }
                                var shopItemDropChance = shopItemDrop["chance"];
                                var parsedShopItemDropChance = -1f;
                                if (shopItemDropChance == null)
                                {
                                    if (followPseudoRandom)
                                    {
                                        _logger.LogError("Error reading item with id {id}. Dropped item {Name} in drop field missing chance field (required for follow_pseudo_rnd).", id, shopItemDrop.Name);
                                        continue;
                                    }
                                }
                                else
                                {
                                    float tempFloat = -1;
                                    if (!float.TryParse(shopItemDropChance.ToString(), out tempFloat) || tempFloat < 0)
                                    {
                                        _logger.LogError("Error reading item with id {id}. Dropped item {Name} in drop field has invalid chance field (positive float expected).", id, shopItemDrop.Name);
                                        continue;
                                    }
                                    parsedShopItemDropChance = tempFloat;
                                }
                                dropList.Add(new ShopItemDrop(parsedShopItemDropID, parsedShopItemDropChance));
                            }
                            if (followPseudoRandom)
                            {
                                var sumOfDropChances = 0f;
                                foreach (var itemDrop in dropList)
                                {
                                    sumOfDropChances += itemDrop.DropChance;
                                }
                                if (sumOfDropChances < 100)
                                {
                                    _logger.LogError("Error reading item with id {id}. Sum of dropped item chances must be equal or greater than 100.", id);
                                    continue;
                                }
                            }
                            else
                            {
                                _logger.LogError("Error reading item with id {id}. Items with drop field defined must have items array defined.", id);
                                continue;
                            }
                        }
                    }
                }
                var readedDuration = shopItem["lifetime"];
                int? itemDuration = null;
                if (readedDuration != null)
                {
                    if (!int.TryParse(readedDuration.ToString(), out int tempInt) || tempInt < 1)
                    {
                        _logger.LogError("Error reading item with id {id}. Invalid \"lifetime\" field (positive integer expected).", id);
                        continue;
                    }
                    else
                    {
                        itemDuration = tempInt;
                    }
                }
                var parsedShopItem = new ShopItem(id, itemName, itemType, isItemBuyable, itemPrice, itemPriceType, followPseudoRandom, dropsCount, dropList.AsReadOnly(), itemDuration);
                if (!_items.TryAdd(id, parsedShopItem))
                {
                    _logger.LogError("Error reading item with id {id}. Item with this id specified more than once.", id);
                    continue;
                }
                else
                {
                    unparsedItems.Add(new UnparsedShopItem(parsedShopItem, shopItem.Where(x => x.Name.Equals("rewards")).FirstOrDefault()));
                }
            }
            // Search for additional items that must be given (subscription heroes, etc)
            foreach (var unparsedShopItem in unparsedItems)
            {
                if (unparsedShopItem.UnparsedAdditionalItems == null)
                {
                    continue;
                }
                foreach (var additionalItem in unparsedShopItem.UnparsedAdditionalItems.Children)
                {
                    if (uint.TryParse(additionalItem.Value.ToString(), out uint id))
                    {
                        var shopItem = GetItemByID(id);
                        if (shopItem == null)
                        {
                            _logger.LogError("Error reading additional item {id} for item with id {itemID} (positive integer expected).", id, unparsedShopItem.Item.ID);
                            continue;
                        }
                        if (!unparsedShopItem.Item.AdditionalItems.TryAdd(shopItem.ID, shopItem))
                        {
                            _logger.LogError("Additional item with id {id} for item with id {itemID} defined more than once.", id, unparsedShopItem.Item.ID);
                        }
                    }
                }
            }
            _logger.LogInformation("Loaded {itemsCount} shop items.", _items.Count);
            _isKeyValuesLoaded = true;
        }

        public bool IsKeyValuesLoaded()
        {
            return _isKeyValuesLoaded;
        }
    }
}
