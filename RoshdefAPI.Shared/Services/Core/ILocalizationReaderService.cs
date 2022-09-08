namespace RoshdefAPI.Shared.Services.Core
{
    public interface IDOTALocalizationService
    {
        public string GetLocalizedString(string key);
        public void LoadKeyValues();
        public bool IsKeyValuesLoaded();
    }
}
