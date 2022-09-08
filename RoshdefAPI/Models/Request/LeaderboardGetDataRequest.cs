using RoshdefAPI.Models.DTO;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class LeaderboardGetDataRequestData
    {
        [JsonPropertyName("players")]
        public List<PlayerSteamIDDTO> Players { get; set; } = new List<PlayerSteamIDDTO>();
        [JsonPropertyName("players_in_leaderboard")]
        public int PlayersInLeaderboard { get; set; } = 100;
    }

    public class LeaderboardGetDataRequest : BaseRequest<LeaderboardGetDataRequestData> {}
}
