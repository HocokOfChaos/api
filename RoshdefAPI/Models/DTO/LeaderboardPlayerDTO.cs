using RoshdefAPI.Data.Constants;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.DTO
{
    public class LeaderboardPlayerDTO
    {
        [JsonPropertyName("steam_id")]
        public string SteamID { get; } = PlayerConstants.InvalidSteamID.ToString();
        [JsonPropertyName("place")]
        public uint Place { get; set; } = 0;
        [JsonPropertyName("points")]
        public ulong Points { get; set; } = 0;

        // Required for auto mapper, dapper, etc
        public LeaderboardPlayerDTO() : this(PlayerConstants.InvalidSteamID.ToString(), 0, 0)
        {

        }
        public LeaderboardPlayerDTO(string steamID, uint place, ulong points) 
        {
            SteamID = steamID;
            Place = place;
            Points = points;
        }
    }
}
