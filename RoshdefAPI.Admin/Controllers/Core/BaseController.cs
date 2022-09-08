using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Admin.Models.Response;

namespace RoshdefAPI.Controllers.Core
{
    public class BaseController : Controller
    {
        private readonly ILogger _logger;

        public BaseController(ILogger logger)
        {
            _logger = logger;
        }
        public T Error<T>(ref T response, string errorMessage) where T : BaseResponse
        {
            _logger.LogError(errorMessage);
            response.Success = false;
            response.Error = errorMessage;
            return response;
        }
    }
}
