using System.Text.Json.Serialization;

namespace RoshdefAPI.Admin.Models.DTO
{
    public class ShopItemDescriptionDTO
    {
        [JsonPropertyName("item_id")]
        public string ItemID { get; set; } = "0";
        [JsonPropertyName("item_name")]
        public string Name { get; set; } = "Tooltip missing";
    }
}
