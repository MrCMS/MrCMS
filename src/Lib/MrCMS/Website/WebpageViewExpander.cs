using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Website
{
    public class WebpageViewExpander : IViewLocationExpander
    {
        private const string Key = "webpage-type";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (!context.IsMainPage)
            {
                return;
            }

            var webpage = context.ActionContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentPage>().GetPage();

            if (webpage != null)
            {
                context.Values[Key] = webpage.DocumentType;
            }
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.ContainsKey(Key))
            {
                // we want to look in the pages folder for the document type
                viewLocations = viewLocations.Prepend($"/Views/Pages/{context.Values[Key]}.cshtml");

                // if we've set the view name, we want to look here for the view
                // it's after the base on because we're prepending
                if (!string.IsNullOrWhiteSpace(context.ViewName))
                {
                    viewLocations = viewLocations.Prepend($"/Views/Pages/{context.ViewName}.cshtml");
                }
            }

            return viewLocations;

        }
    }
}