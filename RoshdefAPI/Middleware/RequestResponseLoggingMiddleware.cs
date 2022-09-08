using Microsoft.Extensions.Options;
using RoshdefAPI.Middleware.Core;
using RoshdefAPI.Shared.Models.Configuration;

namespace RoshdefAPI.Middleware
{
    public class RequestResponseLoggingMiddleware : BaseMiddleware
    {
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly ApplicationSettings _config;

        public RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger, IOptions<ApplicationSettings> config) : base(logger)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async override Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Middleware is enabled only when the EnableRequestResponseLogging config value is set.
            if (_config.EnableRequestResponseLogging && context.Request.Path.ToUriComponent().Contains(_config.BaseAPIPath))
            {
                _logger.LogInformation($"HTTP request information:\n" +
                    $"\tMethod: {context.Request.Method}\n" +
                    $"\tPath: {context.Request.Path}\n" +
                    $"\tQueryString: {context.Request.QueryString}\n" +
                    $"\tHeaders: {FormatHeaders(context.Request.Headers)}\n" +
                    $"\tSchema: {context.Request.Scheme}\n" +
                    $"\tHost: {context.Request.Host}\n" +
                    $"\tBody: {await ReadBodyFromRequest(context.Request)}");

                // Temporarily replace the HttpResponseStream, which is a write-only stream, with a MemoryStream to capture it's value in-flight.
                var originalResponseBody = context.Response.Body;
                using var newResponseBody = new MemoryStream();
                context.Response.Body = newResponseBody;

                // Call the next middleware in the pipeline
                await next(context);

                newResponseBody.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();

                _logger.LogInformation($"HTTP response information:\n" +
                    $"\tStatusCode: {context.Response.StatusCode}\n" +
                    $"\tContentType: {context.Response.ContentType}\n" +
                    $"\tHeaders: {FormatHeaders(context.Response.Headers)}\n" +
                    $"\tBody: {responseBodyText}");

                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalResponseBody);
            }
            else
            {
                await next(context);
            }
        }

        private static string FormatHeaders(IHeaderDictionary headers) => string.Join(", ", headers.Select(kvp => $"{{{kvp.Key}: {string.Join(", ", kvp.Value)}}}"));
    }
}
