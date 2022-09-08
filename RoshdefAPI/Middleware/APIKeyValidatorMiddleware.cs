using Microsoft.Extensions.Options;
using RoshdefAPI.Middleware.Core;
using RoshdefAPI.Models.Request;
using RoshdefAPI.Shared.Models.Configuration;
using RoshdefAPI.Shared.Services.Core;
using System.Text.Json;

namespace RoshdefAPI.Middleware
{
    public class APIKeyValidatorMiddleware : BaseMiddleware
    {

        private readonly IAPIKeyValidator _apiKeyValidator;
        private readonly ILogger<APIKeyValidatorMiddleware> _logger;
        private readonly ApplicationSettings _config;

        public APIKeyValidatorMiddleware(ILogger<APIKeyValidatorMiddleware> logger, IAPIKeyValidator apiKeyValidator, IOptions<ApplicationSettings> config) : base(logger)
        {
            _logger = logger;
            _apiKeyValidator = apiKeyValidator;
            _config = config.Value;
        }

        public async override Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.ToUriComponent().Contains(_config.BaseAPIPath))
            {
                var body = await ReadBodyFromRequest(context.Request);
                BaseRequest? request;
                try
                {
                    request = JsonSerializer.Deserialize<BaseRequest<object>>(body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "JSON is invalid.");
                    await SendErrorResponse(context, "JSON is invalid.");
                    return;
                }
                if (request == null)
                {
                    await SendErrorResponse(context, "Invalid request.");
                    return;
                }
                if (_apiKeyValidator.IsKeyValid(request.APIKey))
                {
                    await next(context);
                }
                else
                {
                    _logger.LogError($"API key is invalid ({request.APIKey}).");
                    await SendErrorResponse(context, "API key is invalid.");
                }
            }
            else
            {
                await next(context);
            }
        }
    }
}
