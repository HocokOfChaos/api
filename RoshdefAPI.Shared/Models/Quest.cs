namespace RoshdefAPI.Shared.Models
{
    public class Quest
    {
        public uint ID { get; private set; }
        public bool IsDaily { get; private set; }

        public Quest(uint id, bool isDaily)
        {
            ID = id;
            IsDaily = isDaily;
        }
    }
}
