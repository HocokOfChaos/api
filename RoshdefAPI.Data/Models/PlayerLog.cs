using RoshdefAPI.Data.Converters;
using RoshdefAPI.Data.Models.Core;
using System.Text.Json;

namespace RoshdefAPI.Data.Models
{
    public class PlayerLog : IDataObject
    {
        public ulong ID { get; set; }
        public string Content { get; }
        public ulong PlayerID { get; }
        public DateTime Date { get; } = DateTime.Now;

        // Required for auto mapper, dapper, etc
        public PlayerLog() : this(new InvalidLogEntry(), 0)
        {

        }

        public PlayerLog(IPlayerLogEntry content, ulong playerID)
        {
            Content = JsonSerializer.Serialize(content, new JsonSerializerOptions
            {
                Converters = { new JsonPlayerLogEntryConverter() }
            });
            PlayerID = playerID;
        }
    }
}
