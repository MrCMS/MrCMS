using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace MrCMS.Website
{
    public class WidgetViewExpander : IViewLocationExpander
    {
        private const string Key = "widget-type";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.IsMainPage)
            {
                return;
            }
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.ContainsKey(Key))
            {
                // we want to look in the pages folder for the document type
                viewLocations = viewLocations.Prepend($"/Views/Widgets/{context.Values[Key]}.cshtml");

                // if we've set the view name, we want to look here for the view
                // it's after the base one because we're prepending
                if (!string.IsNullOrWhiteSpace(context.ViewName))
                {
                    viewLocations = viewLocations.Prepend($"/Views/Pages/{context.ViewName}.cshtml");
                }
            }

            return viewLocations;

        }
    }
}