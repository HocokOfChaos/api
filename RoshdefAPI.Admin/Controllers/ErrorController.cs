using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Admin.Models;
using RoshdefAPI.Admin.Services.Core;

namespace RoshdefAPI.Admin.Controllers
{ 
    public class ErrorController : Controller
    {
        private readonly IJsonStringLocalizer _localizer;
        public ErrorController(IJsonStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        [Route("/Error/HandleStatusCode/{code:int}"), AllowAnonymous]
        public IActionResult HandleStatusCode(int code)
        {
            return View(new StatusCode(code, _localizer));
        }
    }
}
