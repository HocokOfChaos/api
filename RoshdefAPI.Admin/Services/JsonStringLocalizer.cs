using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using RoshdefAPI.Admin.Services.Core;
using RoshdefAPI.Shared.Models.Configuration;
using System.Globalization;
using System.Text.Json;

namespace RoshdefAPI.Admin.Services
{
    public class JsonStringLocalizer : IJsonStringLocalizer
    {

        private readonly Dictionary<string, Dictionary<string, string>> _localization = new();
        private readonly string _pathToDictionary = "";
        private readonly ILogger<JsonStringLocalizer> _logger;
        private readonly LocalizationSettings _config;
        public LocalizedString this[string name] => GetLocalizedString(name);

        public LocalizedString this[string name, params object[] arguments] => GetLocalizedString(name, arguments);

        public JsonStringLocalizer(ILogger<JsonStringLocalizer> logger, IWebHostEnvironment environment, IOptions<LocalizationSettings> options)
        {
            _logger = logger;
            _config = options.Value;
            if (_config.UseRelativePath)
            {
                _pathToDictionary = Path.Combine(environment.ContentRootPath, _config.LocalizationFilesPath);
            }
            else
            {
                _pathToDictionary = _config.LocalizationFilesPath;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var currentCulture = CultureInfo.CurrentUICulture;
            if (_localization.TryGetValue(currentCulture.Name, out var result))
            {
                return result.Select(x => new LocalizedString(x.Key, x.Value));
            }
            return new List<LocalizedString>();
        }

        public LocalizedString GetLocalizedString(string name, params object[] arguments)
        {
            var currentCulture = CultureInfo.CurrentUICulture;
            try
            {
                if (_localization.TryGetValue(currentCulture.Name, out var languageLocalization))
                {
                    if (languageLocalization.TryGetValue(name, out var result))
                    {
                        if (arguments.Length > 0)
                        {
                            return new LocalizedString(name, string.Format(result, arguments));
                        }
                        else
                        {
                            return new LocalizedString(name, result);
                        }
                    }
                }
                else
                {
                    if (_localization.TryGetValue(_config.DefaultCulture.Name, out var defaultLocalization))
                    {
                        if (defaultLocalization.TryGetValue(name, out var result))
                        {
                            if (arguments.Length > 0)
                            {
                                return new LocalizedString(name, string.Format(result, arguments));
                            }
                            else
                            {
                                return new LocalizedString(name, result);
                            }
                        }
                    }
                }
                return new LocalizedString(name, name, true);
            }
            catch (FormatException)
            {
                return GetLocalizedString(name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to localize string {name} with additional data {data}", name, arguments);
                return new LocalizedString(name, name, true);
            }
        }

        public void LoadLocalization()
        {
            try
            {
                foreach (string fileName in Directory.GetFiles(_pathToDictionary, "*.json"))
                {
                    var cultureName = Path.GetFileNameWithoutExtension(fileName);
                    var supportedCulture = _config.SupportedCultureInfos.FirstOrDefault(x => x.Name.Equals(cultureName));

                    if (supportedCulture == null)
                    {
                        _logger.LogError("Skipping {name} localization due to missing entry in {entry_name}", cultureName, nameof(LocalizationSettings.SupportedCultureInfos));
                        continue;
                    }
                    using StreamReader r = new StreamReader(fileName);
                    var loadedStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(r.ReadToEnd());
                    if (loadedStrings != null)
                    {
                        if (!_localization.TryAdd(cultureName, loadedStrings))
                        {
                            _logger.LogError("Attempt to load \"{name}\" localization more than once.", cultureName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception during loading localization...");
            }
            finally
            {
                _logger.LogInformation("Loaded {count} localized strings.", _localization.Values.Sum(languageLocalization => languageLocalization.Count));
            }
        }
    }
}
