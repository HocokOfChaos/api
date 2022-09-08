using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Response;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class FinishMatchResponseData
    {
        [JsonPropertyName("players")]
        public Dictionary<ulong, PlayerBalanceDTO> Players { get; set; } = new();
    }
    public class FinishMatchResponse : BaseResponse<FinishMatchResponseData> { }
}
