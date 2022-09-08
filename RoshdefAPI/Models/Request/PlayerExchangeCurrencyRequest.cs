using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class PlayerExchangeCurrencyRequestData
    {
        [JsonPropertyName("steam_id")]
        public ulong SteamID { get; set; } = 0;
        [JsonPropertyName("crystals")]
        public int CrystalsForExchange { get; set; } = 0;
    }

    public class PlayerExchangeCurrencyRequest : BaseRequest<PlayerExchangeCurrencyRequestData> { }
}
