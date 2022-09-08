using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.DTO
{
    public class FinishMatchDressedItemDTO
    {
        [JsonPropertyName("id")]
        public ulong ItemID { get; set; }
    }
}
