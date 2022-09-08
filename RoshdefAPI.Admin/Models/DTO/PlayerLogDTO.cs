using System.Text.Json.Serialization;

namespace RoshdefAPI.Admin.Models.DTO
{
    public class PlayerLogDTO
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = "Tooltip missing.";
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }
}
