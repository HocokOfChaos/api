using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class ShopBuyItemRequestData
    {
        [JsonPropertyName("steam_id")]
        public ulong SteamID { get; set; } = 0;
        [JsonPropertyName("item_id")]
        public uint ItemID { get; set; } = 0;
    }

    public class ShopBuyItemRequest : BaseRequest<ShopBuyItemRequestData> { }
}
