using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoshdefAPI.Shared.Models.Configuration;
using RoshdefAPI.Shared.Services.Core;
using ValveKeyValue;

namespace RoshdefAPI.Shared.Services
{
    public class DOTALocalizationReaderService : IDOTALocalizationService
    {
        private readonly ILogger<DOTALocalizationReaderService> _logger;
        private readonly Dictionary<string, string> _localization;
        private readonly string _pathToLocalization;
        private bool _isKeyValuesLoaded = false;

        public DOTALocalizationReaderService(ILogger<DOTALocalizationReaderService> logger, IOptions<ApplicationSettings> config)
        {
            _logger = logger;
            _pathToLocalization = config.Value.PathToLocalization;
            if (config.Value.UseRelativePathForLocalization)
            {
                _pathToLocalization = Path.Combine(Directory.GetCurrentDirectory(), _pathToLocalization);
            }
            _localization = new Dictionary<string, string>();
        }

        public string GetLocalizedString(string key)
        {
            if (_localization.TryGetValue(key, out var item))
            {
                return item;
            }
            else
            {
                return key;
            }
        }

        public bool IsKeyValuesLoaded()
        {
            return _isKeyValuesLoaded;
        }

        public void LoadKeyValues()
        {
            if (IsKeyValuesLoaded())
            {
                _logger.LogError("Attempt to load key values for localization more than once.");
                return;
            }
            var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            KVObject kv;
            using (var stream = new FileStream(_pathToLocalization, FileMode.Open, FileAccess.Read))
            {
                kv = serializer.Deserialize(stream);
            }
            var localizationStrings = kv.Children.Where(x => x.Name.Equals("Tokens")).FirstOrDefault();
            if (localizationStrings == null)
            {
                throw new Exception("Can't find Tokens key in specified localization file.");
            }
            foreach (var localizationEntry in localizationStrings.Children)
            {
                var key = localizationEntry.Name;
                var value = localizationEntry.Value.ToString();
                if (value == null)
                {
                    _logger.LogError("Failed to load localized string for key {key}.", key);
                }
                else
                {
                    if (!_localization.TryAdd(key, value))
                    {
                        _localization[key] = value;
                    }
                }
            }
            _isKeyValuesLoaded = true;
            _logger.LogInformation("Loaded {itemsCount} localized strings.", _localization.Count);
        }
    }
}
