using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Services
{
    public interface IModelBindingHelperAdaptor
    {
        Task<bool> TryUpdateModelAsync<TModel>(ControllerBase controller, TModel model) where TModel : class;
        Task<bool> TryUpdateModelAsync(ControllerBase controller, object model, Type type, string prefix = null);
    }
}