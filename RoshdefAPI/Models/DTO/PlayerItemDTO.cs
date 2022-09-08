using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.DTO
{
    public class PlayerItemDTO
    {
        [JsonPropertyName("id")]
        public ulong ItemID { get; set; }
        [JsonPropertyName("count")]
        public uint Count { get; set; } = 1;
        [JsonPropertyName("is_dressed"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsDressed { get; set; } = null;
        [JsonPropertyName("expire_date"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? ExpireDate { get; set; } = null;
    }
}
