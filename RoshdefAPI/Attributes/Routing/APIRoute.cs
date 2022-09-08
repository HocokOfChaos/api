using Microsoft.AspNetCore.Mvc.Routing;
using RoshdefAPI.Shared.Models.Configuration;

namespace RoshdefAPI.Attributes.Routing
{
    public class APIRoute : Attribute, IRouteTemplateProvider
    {
        public string? Template { get; set; }

        public int? Order => 0;

        public string? Name { get; set; }

        public APIRoute(string? template)
        {
            // idk how to pass IOptions<ApplicationSettings> here
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var Configuration = builder.Build();

            var config = Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
            Template = config.BaseAPIPath + template;
        }
    }
}
