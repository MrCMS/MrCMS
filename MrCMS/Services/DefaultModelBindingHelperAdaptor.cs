using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Services
{
    public class DefaultModelBindingHelperAdaptor : IModelBindingHelperAdaptor
    {
        public virtual Task<bool> TryUpdateModelAsync<TModel>(ControllerBase controller, TModel model) where TModel : class
        {
            return controller.TryUpdateModelAsync(model);
        }

        public Task<bool> TryUpdateModelAsync(ControllerBase controller, object model, Type type, string prefix = null)
        {
            return controller.TryUpdateModelAsync(model, type, prefix ?? string.Empty);
        }
    }
}