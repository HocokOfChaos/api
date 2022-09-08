using RoshdefAPI.Data.Constants;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.DTO
{
    public class PlayerDTO
    {
        [JsonPropertyName("steam_id")]
        public string SteamID { get; set; } = PlayerConstants.InvalidSteamID.ToString();
        [JsonPropertyName("items")]
        public List<PlayerItemDTO> Items { get; set; } = new List<PlayerItemDTO>();
        [JsonPropertyName("balance")]
        public PlayerBalanceDTO Balance { get; set; } = new PlayerBalanceDTO();
        [JsonPropertyName("payment")]
        public string? Payment { get; set; }
        [JsonPropertyName("is_daily_quest_available")]
        public bool IsDailyQuestAvailable { get; set; } = false;
        [JsonPropertyName("is_daily_reward_available")]
        public bool IsDailyRewardAvailable { get; set; } = false;
        [JsonPropertyName("current_daily_reward_day")]
        public int CurrentDailyRewardDay { get; set; } = 0;
    }
}
