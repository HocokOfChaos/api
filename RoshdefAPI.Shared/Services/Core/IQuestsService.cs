using RoshdefAPI.Shared.Models;

namespace RoshdefAPI.Shared.Services.Core
{
    public interface IQuestsService
    {
        public Quest? GetQuestByID(uint id);
        public IEnumerable<Quest> GetAllDailyQuests();
        public void LoadKeyValues();
        public bool IsKeyValuesLoaded();
    }
}
