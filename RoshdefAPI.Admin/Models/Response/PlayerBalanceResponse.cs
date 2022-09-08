using RoshdefAPI.Admin.Models.DTO;
using RoshdefAPI.Admin.Models.Response;
using RoshdefAPI.Data.Constants;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Admin.Models.Request
{
    public class PlayerBalanceResponseContent
    {
        [JsonPropertyName("steam_id")]
        public string SteamID { get; set; } = PlayerConstants.InvalidSteamID.ToString();
        [JsonPropertyName("balance")]
        public PlayerBalanceDTO Balance { get; set; } = new();
    }

    public class PlayerBalanceResponse : BaseResponse<PlayerBalanceResponseContent> { }
}
