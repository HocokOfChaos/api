using RoshdefAPI.Data.Constants;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Admin.Models.DTO
{
    public class PlayerItemDTO
    {
        [JsonPropertyName("item_id")]
        public string ItemID { get; set; } = "0";
        [JsonPropertyName("item_name")]
        public string Name { get; set; } = "Tooltip missing";
        [JsonPropertyName("count")]
        public uint Count { get; set; } = 0;
        [JsonPropertyName("expire_date")]
        public DateTime? ExpireDate { get; set; } = null;
    }
}
