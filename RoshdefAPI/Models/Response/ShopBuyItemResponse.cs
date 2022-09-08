using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Response;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class ShopBuyItemResponseData
    {
        [JsonPropertyName("items")]
        public List<PlayerItemDTO> Items { get; set; } = new List<PlayerItemDTO>();
        [JsonPropertyName("balance")]
        public PlayerBalanceDTO Balance { get; set; } = new PlayerBalanceDTO();
    }

    public class ShopBuyItemResponse : BaseResponse<ShopBuyItemResponseData> { }
}
