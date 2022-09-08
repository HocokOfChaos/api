namespace RoshdefAPI.Shared.Models.Configuration
{
    public class ApplicationSettings
    {
        public string DedicatedServerKey { get; set; } = "dota";
        public bool EnableRequestResponseLogging { get; set; } = true;
        public bool UseHTTPS { get; set; } = false;
        public int HTTPSPort { get; set; } = 443;
        public bool UseRelativePathForKV { get; set; } = true;
        public string PathToShopItemsKV { get; set; } = "KV/shop_items.kv";
        public string PathToQuestsKV { get; set; } = "KV/quests.kv";
        public bool UseRelativePathForLocalization { get; set; } = true;
        public string PathToLocalization { get; set; } = "";
        public string BaseAPIPath { get; set; } = "/api";
        public bool UseForwardedHeaders { get; set; } = false;
        public bool RegistrationEnabled { get; set; } = false;
    }
}
