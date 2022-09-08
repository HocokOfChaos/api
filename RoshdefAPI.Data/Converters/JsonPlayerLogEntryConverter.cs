using RoshdefAPI.Data.Models.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoshdefAPI.Data.Converters
{
    public class JsonPlayerLogEntryConverter : JsonConverter<IPlayerLogEntry>
    {
        public override IPlayerLogEntry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IPlayerLogEntry value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    JsonSerializer.Serialize(writer, (IPlayerLogEntry)null, options);
                    break;
                default:
                    {
                        var type = value.GetType();
                        JsonSerializer.Serialize(writer, value, type, options);
                        break;
                    }
            }
        }
    }
}
