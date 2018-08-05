using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class WebpageViewExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (!context.IsMainPage) return;
            var webpage = context.ActionContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentPage>().GetPage();

            if (webpage != null)
                context.Values["webpage"] = webpage.DocumentType;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (!context.Values.ContainsKey("webpage"))
                return viewLocations;

            return viewLocations.Prepend($"/Views/Pages/{context.Values["webpage"]}.cshtml");
        }
    }
}