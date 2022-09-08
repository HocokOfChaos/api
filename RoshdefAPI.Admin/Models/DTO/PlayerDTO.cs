using RoshdefAPI.Data.Constants;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Admin.Models.DTO
{
    public class PlayerDTO
    {
        [JsonPropertyName("steam_id")]
        public string SteamID { get; set; } = PlayerConstants.InvalidSteamID.ToString();
        [JsonPropertyName("items")]
        public List<PlayerItemDTO> Items { get; set; } = new();
        [JsonPropertyName("balance")]
        public PlayerBalanceDTO Balance { get; set; } = new();

        // Required for auto mapper, dapper, etc
        public PlayerDTO() : this(PlayerConstants.InvalidSteamID)
        {

        }

        public PlayerDTO(ulong steamID)
        {
            SteamID = steamID.ToString();
        }
    }
}
