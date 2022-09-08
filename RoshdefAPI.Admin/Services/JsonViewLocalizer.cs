using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using RoshdefAPI.Admin.Services.Core;

namespace RoshdefAPI.Admin.Services
{
    public class JsonViewLocalizer : IJsonViewLocalizer
    {
        private readonly IJsonStringLocalizer _localizer;

        public JsonViewLocalizer(IJsonStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        public LocalizedHtmlString this[string name] => new LocalizedHtmlString(name, GetString(name));

        public LocalizedHtmlString this[string name, params object[] arguments] => new LocalizedHtmlString(name, GetString(name, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }

        public LocalizedString GetString(string name)
        {
            return _localizer.GetLocalizedString(name);
        }

        public LocalizedString GetString(string name, params object[] arguments)
        {
            return _localizer.GetLocalizedString(name, arguments);
        }
    }
}
