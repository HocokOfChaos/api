using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.DTO
{
    public class FinishMatchConsumedItemDTO
    {
        [JsonPropertyName("id")]
        public ulong ItemID { get; set; }
        [JsonPropertyName("count")]
        public uint Count { get; set; } = 1;
    }
}
