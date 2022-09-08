using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Models.Response;

namespace RoshdefAPI.Controllers.Core
{
    public class APIController : ControllerBase
    {
        private readonly ILogger _logger;

        public APIController(ILogger logger)
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
