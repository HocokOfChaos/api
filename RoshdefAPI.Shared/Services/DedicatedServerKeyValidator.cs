using Microsoft.Extensions.Options;
using RoshdefAPI.Shared.Models.Configuration;
using RoshdefAPI.Shared.Services.Core;

namespace RoshdefAPI.Shared.Services
{
    public class DedicatedServerKeyValidator : IAPIKeyValidator
    {
        private readonly ApplicationSettings _config;

        public DedicatedServerKeyValidator(IOptions<ApplicationSettings> config)
        {
            _config = config.Value;
        }

        public bool IsKeyValid(string apiKey)
        {
            apiKey ??= "invalid_api_key_3123qwesdadqdqewe_1";
            return apiKey.Equals(_config.DedicatedServerKey);
        }
    }
}
