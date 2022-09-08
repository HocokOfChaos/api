using RoshdefAPI.Models.DTO;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Response
{
    public class DailyRewardConfirmResponseData
    {
        [JsonPropertyName("items")]
        public List<PlayerItemDTO> Items { get; set; } = new List<PlayerItemDTO>();
        [JsonPropertyName("balance")]
        public PlayerBalanceDTO Balance { get; set; } = new PlayerBalanceDTO();
    }

    public class DailyRewardConfirmResponse : BaseResponse<DailyRewardConfirmResponseData> { };
}
