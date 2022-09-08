using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace RoshdefAPI.Admin.Services.Core
{
    public interface IViewRenderService
    {
        public Task<string> Render(string name, ViewDataDictionary viewDataDictionary = null);
        public Task<string> Render<TModel>(string name, TModel model, ViewDataDictionary viewDataDictionary = null);
    }

}