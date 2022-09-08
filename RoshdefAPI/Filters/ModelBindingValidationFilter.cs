using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RoshdefAPI.Models.Response;
using System.Text.Json;
using System.Net.Mime;

namespace RoshdefAPI.Filters
{
    public class ModelBindingValidationFilter : IAsyncActionFilter
    {
        private readonly ILogger<ModelBindingValidationFilter> _logger;

        public ModelBindingValidationFilter(ILogger<ModelBindingValidationFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray());

                foreach (var error in errorsInModelState)
                {
                    if (error.Value != null)
                    {
                        foreach (var subError in error.Value)
                        {
                            _logger.LogError($"{error.Key} {subError}");
                        }
                    }
                }
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = MediaTypeNames.Application.Json,
                    Content = GetErrorResponse()
                };
                return;
            }

            await next();
        }

        public string GetErrorResponse(string errorText = "A validation error has occurred while executing the request")
        {
            try
            {
                var json = JsonSerializer.Serialize(new InternalErrorResponse { Success = false, Error = errorText });
                return json;
            }
            catch (Exception ex)
            {
                const string mes = "An exception has occurred while serializing error to JSON";
                _logger.LogError(ex, mes);
            }
            return string.Empty;
        }
    }
}
