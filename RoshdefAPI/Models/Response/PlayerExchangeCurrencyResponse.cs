using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Response;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class PlayerExchangeCurrencyResponseData
    {
        [JsonPropertyName("balance")]
        public PlayerBalanceDTO Balance { get; set; } = new PlayerBalanceDTO();
    }

    public class PlayerExchangeCurrencyResponse : BaseResponse<PlayerExchangeCurrencyResponseData> { }
}
