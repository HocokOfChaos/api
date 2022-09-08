using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoshdefAPI.Shared.Models;
using RoshdefAPI.Shared.Models.Configuration;
using RoshdefAPI.Shared.Services.Core;
using ValveKeyValue;

namespace RoshdefAPI.Shared.Services
{
    public class QuestsService : IQuestsService
    {
        private readonly ILogger<QuestsService> _logger;
        private readonly HashSet<Quest> _quests;
        private readonly string _pathToKV;
        private bool _isKeyValuesLoaded = false;

        public QuestsService(ILogger<QuestsService> logger, IOptions<ApplicationSettings> config)
        {
            _logger = logger;

            _pathToKV = config.Value.PathToQuestsKV;
            if(config.Value.UseRelativePathForKV)
            {
                _pathToKV = Path.Combine(Directory.GetCurrentDirectory(), _pathToKV);
            }
            _quests = new HashSet<Quest>();
        }
        public IEnumerable<Quest> GetAllDailyQuests()
        {
            return _quests.Where(x => x.IsDaily == true);
        }

        public Quest? GetQuestByID(uint id)
        {
            return _quests.Where(x => x.ID.Equals(id)).FirstOrDefault();
        }

        public bool IsKeyValuesLoaded()
        {
            return _isKeyValuesLoaded;
        }

        public void LoadKeyValues()
        {
            if (IsKeyValuesLoaded())
            {
                _logger.LogError("Attempt to load key values for quests more than once.");
                return;
            }
            var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            KVObject kv;
            using (var stream = new FileStream(_pathToKV, FileMode.Open, FileAccess.Read))
            {
                kv = serializer.Deserialize(stream);
            }
            foreach (var quest in kv.Children)
            {
                if (!uint.TryParse(quest.Name.ToString(), out uint id))
                {
                    _logger.LogError("Error reading quest with id {id} (positive integer expected).", id);
                    continue;
                }
                bool isDaily = false;
                var readedDaily = quest["daily"];
                if (readedDaily != null)
                {
                    if (!int.TryParse(readedDaily.ToString(), out int tempInt))
                    {
                        _logger.LogError("Error reading daily field of quest with id {id} (positive integer expected).", id);
                        continue;
                    }
                    isDaily = tempInt == 1;
                }
                var parsedQuest = new Quest(id, isDaily);
                var isAdded = _quests.Add(parsedQuest);
                if (!isAdded)
                {
                    _logger.LogError("Error reading quest with id {id}. Quest with this id specified more than once.", id);
                }
            }
            _logger.LogInformation("Loaded {questsCount} quests.", _quests.Count);
            _isKeyValuesLoaded = true;
        }
    }
}
