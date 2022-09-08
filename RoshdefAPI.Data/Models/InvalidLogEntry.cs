using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class InvalidLogEntry : IPlayerLogEntry
    {
        public IPlayerLogEntry.Type LogType => IPlayerLogEntry.Type.Invalid;
    }
}
