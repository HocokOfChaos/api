using RoshdefAPI.Data.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Models.DTO
{
    public class FinishMatchPlayerDTO
    {
        [JsonPropertyName("steam_id")]
        public ulong SteamID { get; set; } = 0;
        [JsonPropertyName("gold_per_minute")]
        public uint GoldPerMinute { get; set; }
        [JsonPropertyName("expirience_per_minute")]
        public uint ExpiriencePerMinute { get; set; }
        [JsonPropertyName("networth")]
        public ulong Networth { get; set; }
        [JsonPropertyName("creeps_killed")]
        public uint CreepsKilled { get; set; }
        [JsonPropertyName("bosses_killed")]
        public uint BossesKilled { get; set; }
        [JsonPropertyName("damage_done")]
        public ulong DamageDone { get; set; }
        [JsonPropertyName("damage_taken")]
        public ulong DamageTaken { get; set; }
        [JsonPropertyName("healing_done")]
        public ulong HealingDone { get; set; }
        [JsonPropertyName("lifesteal_done")]
        public ulong LifestealDone { get; set; }
        [JsonPropertyName("quests_finished")]
        public string QuestsFinished { get; set; } = JsonSerializer.Serialize(new Dictionary<string, string>());
        [JsonPropertyName("items_build")]
        public string ItemsBuild { get; set; } = JsonSerializer.Serialize(new Dictionary<string, string>());
        [JsonPropertyName("consumed_items")]
        public List<FinishMatchConsumedItemDTO> ConsumedItems { get; set; } = new List<FinishMatchConsumedItemDTO>();
        [JsonPropertyName("dressed_items")]
        public List<FinishMatchDressedItemDTO> DressedItems { get; set; } = new List<FinishMatchDressedItemDTO>();
    }
}
