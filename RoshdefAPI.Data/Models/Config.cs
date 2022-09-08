using RoshdefAPI.Data.Models.Core;

namespace RoshdefAPI.Data.Models
{
    public class Config : IDataObject
    {
        public ulong ID { get; set; }
        public enum ConfigType
        {
            DailyQuestID = 1,
            CrystalsToCoinsExchangeRate = 2
        }

        public ConfigType Type { get; }
        public string Value { get; set; } = string.Empty;
    }
}
