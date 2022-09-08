using RoshdefAPI.Data.Constants;
using RoshdefAPI.Models.DTO;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.Response
{
    public class BeginMatchResponseData
    {
        [JsonPropertyName("match_id")]
        public ulong MatchID { get; set; } = MatchConstants.InvalidMatchID;
        [JsonPropertyName("daily_quest_id")]
        public uint DailyQuestID { get; set; } = QuestsConstants.InvalidDailyQuestID;
        [JsonPropertyName("players")]
        public List<PlayerDTO> Players { get; set; } = new List<PlayerDTO>();
        [JsonPropertyName("last_daily_reward_day")]
        public uint LastDailyRewardDay { get; set; } = DailyRewardConstants.InvalidLastDailyRewardDay;
    }
    public class BeginMatchResponse : BaseResponse<BeginMatchResponseData> { };
}
