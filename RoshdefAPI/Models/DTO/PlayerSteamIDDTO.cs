using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.DTO
{
    public class PlayerSteamIDDTO
    {
        [JsonPropertyName("steam_id")]
        public ulong SteamID { get; set; }
    }
}
