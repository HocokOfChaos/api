using RoshdefAPI.Data.Constants;
using RoshdefAPI.Models.DTO;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class BeginMatchRequestData
    {
        [JsonPropertyName("players")]
        public List<PlayerSteamIDDTO> Players { get; set; } = new List<PlayerSteamIDDTO>();
        [JsonPropertyName("dota_match_id")]
        public ulong DotaMatchID { get; set; } = MatchConstants.InvalidDOTAMatchID;
    }

    public class BeginMatchRequest : BaseRequest<BeginMatchRequestData> { }
}
