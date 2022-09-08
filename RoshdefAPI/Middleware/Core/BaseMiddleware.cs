using RoshdefAPI.Models.Response;
using System.Net.Mime;
using System.Text.Json;

namespace RoshdefAPI.Middleware.Core
{
    public abstract class BaseMiddleware : IMiddleware
    {
        private readonly ILogger<BaseMiddleware> _logger;

        public BaseMiddleware(ILogger<BaseMiddleware> logger)
        {
            _logger = logger;
        }

        public virtual Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }

        public async Task SendErrorResponse(HttpContext context, string errorText = "An unhandled exception has occurred while executing the request")
        {
            try
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                var json = JsonSerializer.Serialize(new InternalErrorResponse { Success = false, Error = errorText });
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                const string mes = "An exception has occurred while serializing error to JSON";
                _logger.LogError(ex, mes);
            }
            await context.Response.WriteAsync(string.Empty);
        }

        public async Task<string> ReadBodyFromRequest(HttpRequest request)
        {
            // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).
            request.EnableBuffering();

            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
            var requestBody = await streamReader.ReadToEndAsync();

            // Reset the request's body stream position for next middleware in the pipeline.
            request.Body.Position = 0;
            return requestBody;
        }
    }
}
