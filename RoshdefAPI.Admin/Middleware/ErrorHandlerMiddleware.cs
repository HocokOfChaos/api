using RoshdefAPI.Admin.Models;
using RoshdefAPI.Admin.Services.Core;
using System.Net;

namespace RoshdefAPI.Admin.Middleware
{
    public class ErrorHandlerMiddleware : IMiddleware
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly IJsonStringLocalizer _localizer;

        public ErrorHandlerMiddleware(IViewRenderService viewRenderService, IJsonStringLocalizer localizer)
        {
            _viewRenderService = viewRenderService;
            _localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (TaskCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                context.Response.Clear();
                context.Response.StatusCode = 499; //Client Closed Request
            }
            catch (Exception)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                var text = await _viewRenderService.Render("Error/HandleInternalError", new StatusCode(500, _localizer));
                await context.Response.WriteAsync(text);
            }
        }
    }
}
