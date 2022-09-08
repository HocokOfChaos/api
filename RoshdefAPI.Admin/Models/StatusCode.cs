using RoshdefAPI.Admin.Services.Core;

namespace RoshdefAPI.Admin.Models
{
    public class StatusCode
    {
        public int Code { get; set; } = 500;

        private readonly IJsonStringLocalizer _localizer;
        public StatusCode(int statusCode, IJsonStringLocalizer localizer)
        {
            Code = statusCode;
            _localizer = localizer;
        }

        public string GetDescription()
        {
            if (Code == 404)
            {
                return _localizer["ErrorPage.Container.404.Description"];
            }
            else
            {
                return _localizer["ErrorPage.Container.Default.Description"];
            }
        }
    }
}
