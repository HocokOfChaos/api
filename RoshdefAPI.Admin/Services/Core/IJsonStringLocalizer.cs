using Microsoft.Extensions.Localization;

namespace RoshdefAPI.Admin.Services.Core
{
    public interface IJsonStringLocalizer : IStringLocalizer
    {
        public void LoadLocalization();
        public LocalizedString GetLocalizedString(string name, params object[] arguments);
    }
}