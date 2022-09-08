using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class ShopOpenTreasureRequestData
    {
        [JsonPropertyName("steam_id")]
        public ulong SteamID { get; set; } = 0;
        [JsonPropertyName("item_id")]
        public uint ItemID { get; set; } = 0;
        [JsonPropertyName("count")]
        public uint Count { get; set; } = 1;
    }

    public class ShopOpenTreasureRequest : BaseRequest<ShopOpenTreasureRequestData> { }
}
