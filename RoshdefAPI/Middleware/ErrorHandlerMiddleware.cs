using RoshdefAPI.Middleware.Core;

namespace RoshdefAPI.Middleware
{
    public class ErrorHandlerMiddleware : BaseMiddleware
    {

        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger) : base(logger)
        {
            _logger = logger;
        }

        public async override Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (TaskCanceledException exception) when (context.RequestAborted.IsCancellationRequested)
            {
                const string message = "Request was cancelled";
                _logger.LogInformation(message);
                _logger.LogDebug(exception, message);

                context.Response.Clear();
                context.Response.StatusCode = 499; //Client Closed Request
            }
            catch (Exception exception)
            {
                string message = String.Format("An unhandled exception has occurred while executing the request ({0}).", context.TraceIdentifier);
                _logger.LogError(exception, message);
                await SendErrorResponse(context, message);
            }
        }
    }
}
