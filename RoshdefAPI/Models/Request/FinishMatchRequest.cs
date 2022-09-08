using RoshdefAPI.Data.Constants;
using RoshdefAPI.Models.DTO;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class FinishMatchRequestData
    {
        [JsonPropertyName("match_id")]
        public uint MatchID { get; set; }
        [JsonPropertyName("dota_match_id")]
        public ulong DotaMatchID { get; set; } = MatchConstants.InvalidDOTAMatchID;
        [JsonPropertyName("diffculty")]
        public uint Difficulty { get; set; } = MatchConstants.ExplorationDifficulty;
        [JsonPropertyName("winner")]
        public uint Winner { get; set; } = MatchConstants.UnknownWinner;
        [JsonPropertyName("duration")]
        public uint Duration { get; set; } = 0;
        [JsonPropertyName("players")]
        public Dictionary<string, FinishMatchPlayerDTO> Players { get; set; } = new Dictionary<string, FinishMatchPlayerDTO>();
    }

    public class FinishMatchRequest : BaseRequest<FinishMatchRequestData> { }
}
