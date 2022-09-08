using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RoshdefAPI.Admin.Services.Core;
using System.Text.RegularExpressions;

namespace RoshdefAPI.Admin.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly ITempDataProvider _tempDataProvider;
        private readonly HttpContext _context;
        private readonly ICompositeViewEngine _viewEngine;
        private const string ControllerStr = "controller";

        public ViewRenderService(
            ITempDataProvider tempDataProvider,
            IHttpContextAccessor accessor,
            ICompositeViewEngine viewEngine)
        {
            _tempDataProvider = tempDataProvider;
            _context = accessor.HttpContext;
            _viewEngine = viewEngine;
        }

        public async Task<string> Render<TModel>(string viewName, TModel model, ViewDataDictionary viewDataDictionary = null)
        {
            return await RenderInternal(viewName, model, viewDataDictionary);
        }
        public async Task<string> Render(string viewName, ViewDataDictionary viewDataDictionary = null)
        {
            return await RenderInternal(viewName, string.Empty, viewDataDictionary);
        }

        private async Task<string> RenderInternal<TModel>(string viewName, TModel model, ViewDataDictionary viewDataDictionary)
        {
            var controller = string.Empty;
            viewName = viewName?.TrimStart(new char[] { '/' });
            Regex rex = new Regex(@"^(\w+)\/(.*)$");
            Match match = rex.Match(viewName);
            if (match.Success)
            {
                controller = match.Groups[1].Value;
                viewName = match.Groups[2].Value;
            }

            var routeData = new RouteData();
            routeData.Values.Add(ControllerStr, controller);
            var actionContext = new ActionContext(_context, routeData, new ActionDescriptor());

            var viewResult = _viewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
            {
                Console.WriteLine($"Searched the following locations: {string.Join(", ", viewResult.SearchedLocations)} for folder \"{controller}\" and view \"{viewName}\"");
                throw new ArgumentNullException($"{viewName} does not match any available view");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            if (viewDataDictionary != null)
            {
                foreach (var obj in viewDataDictionary)
                {
                    viewDictionary.Add(obj);
                }
            }

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(_context, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );
                viewContext.RouteData = _context.GetRouteData();

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}