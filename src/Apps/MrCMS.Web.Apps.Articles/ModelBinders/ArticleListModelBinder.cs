using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Web.Apps.Articles.Models;

namespace MrCMS.Web.Apps.Articles.ModelBinders
{
    public class ArticleListModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            int.TryParse(bindingContext.ValueProvider.GetValue("page").FirstValue, out var page);
            if (page == 0)
                page = 1;

            int? month = int.TryParse(bindingContext.ValueProvider.GetValue("month").FirstValue, out int monthVal)
                ? monthVal
                : (int?) null;

            int? year = int.TryParse(bindingContext.ValueProvider.GetValue("year").FirstValue, out var yearVal)
                ? yearVal
                : (int?) null;

            bindingContext.Result = ModelBindingResult.Success(new ArticleSearchModel
            {
                Page = page,
                Category = bindingContext.ValueProvider.GetValue("category").FirstValue,
                Month = month,
                Year = year
            });

            return Task.CompletedTask;
        }
    }
}
