using System.Globalization;

namespace RoshdefAPI.Shared.Models.Configuration
{
    public class LocalizationSettings   
    {
        private const string DEFAULT_CULTURE = "ru-RU";

        private CultureInfo defaultCulture = new CultureInfo(DEFAULT_CULTURE);

        /// <summary>
        /// Sets the default culture to use.
        /// </summary>
        public CultureInfo DefaultCulture
        {
            get => defaultCulture;
            set
            {
                if (value != defaultCulture)
                {
                    defaultCulture = value ?? CultureInfo.InvariantCulture;
                }
            }
        }

        private HashSet<CultureInfo> supportedCultureInfos = new HashSet<CultureInfo>
        {

        };

        /// <summary>
        /// Optional array of cultures that you should provide to plugin. (Like RequestLocalizationOptions)
        /// </summary>
        public HashSet<CultureInfo> SupportedCultureInfos
        {
            get => supportedCultureInfos;
            set
            {
                if (value != supportedCultureInfos)
                {
                    supportedCultureInfos = value;
                }
            }
        }

        public string LocalizationFilesPath { get; set; } = "";
        public bool UseRelativePath { get; set; } = true;
    }
}
