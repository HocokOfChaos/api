using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.DTO
{
    public class PlayerBalanceDTO
    {
        [JsonPropertyName("crystals")]
        public int Crystals { get; set; } = 0;
        [JsonPropertyName("coins")]
        public int Coins { get; set; } = 0;
        [JsonPropertyName("soul_stones")]
        public int SoulStones { get; set; } = 0;
    }
}
