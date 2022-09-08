using RoshdefAPI.Models.DTO;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Response
{
    public class LeaderboardGetDataResponseData
    {
        [JsonPropertyName("day")]
        public List<LeaderboardPlayerDTO> Day { get; set; } = new List<LeaderboardPlayerDTO>();
        [JsonPropertyName("week")]
        public List<LeaderboardPlayerDTO> Week { get; set; } = new List<LeaderboardPlayerDTO>();
        [JsonPropertyName("month")]
        public List<LeaderboardPlayerDTO> Month { get; set; } = new List<LeaderboardPlayerDTO>();
    }

    public class LeaderboardGetDataResponse : BaseResponse<Dictionary<ulong, LeaderboardGetDataResponseData>> { }
}
