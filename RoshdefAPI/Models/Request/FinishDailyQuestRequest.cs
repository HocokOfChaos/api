using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class FinishDailyQuestRequestData
    {
        [JsonPropertyName("steam_id")]
        public ulong SteamID { get; set; } = 0;
        [JsonPropertyName("crystals")]
        public int Crystals { get; set; } = 0;
        [JsonPropertyName("coins")]
        public int Coins { get; set; } = 0;
        [JsonPropertyName("soul_stones")]
        public int SoulStones { get; set; } = 0;
        [JsonPropertyName("match_id")]
        public uint MatchID { get; set; }
    }

    public class FinishDailyQuestRequest : BaseRequest<FinishDailyQuestRequestData> { }
}
