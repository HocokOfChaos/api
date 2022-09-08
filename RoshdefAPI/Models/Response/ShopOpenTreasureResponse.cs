using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Response;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Request
{
    public class ShopOpenTreasureResponseData
    {
        [JsonPropertyName("items")]
        public List<PlayerItemDTO> Items { get; set; } = new List<PlayerItemDTO>();
    }

    public class ShopOpenTreasureResponse : BaseResponse<ShopOpenTreasureResponseData> { }
}
