using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Response;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class FinishDailyQuestResponseData
    {
        [JsonPropertyName("balance")]
        public PlayerBalanceDTO Balance { get; set; } = new PlayerBalanceDTO();
    }

    public class FinishDailyQuestResponse : BaseResponse<FinishDailyQuestResponseData> { }
}
