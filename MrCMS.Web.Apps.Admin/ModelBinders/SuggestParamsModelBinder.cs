using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class SuggestParamsModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelBinder = new SimpleTypeModelBinder(typeof(SuggestParams));
            await modelBinder.BindModelAsync(bindingContext);
            var model = bindingContext.Result.Model as SuggestParams;
            if (model == null) return;

            var documentType = model.DocumentType;
            var parts = documentType.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return;
            }
            model.DocumentType = parts[0];
            model.Template = int.TryParse(parts[1], out var id) ? id : (int?)null;

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}