using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Web.Apps.Core.Models.Search;

namespace MrCMS.Web.Apps.Core.ModelBinders
{
    public class WebpageSearchQueryModelBinder : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            int.TryParse(bindingContext.ValueProvider.GetValue("page").FirstValue, out var page);
            if (page == 0)
                page = 1;
            bindingContext.Result = ModelBindingResult.Success(new WebpageSearchQuery
            {
                Page = page,
                Term = bindingContext.ValueProvider.GetValue("term").FirstValue
            });

            return Task.CompletedTask;
        }
    }
}