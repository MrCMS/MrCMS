using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace MrCMS.Website
{
    public class AppViewLocationExpander : IViewLocationExpander
    {
        private const string Key = "app";

        // TODO: implement properly
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var value = context.ActionContext.RouteData.DataTokens[Key];
            if (value != null)
            {
                context.Values[Key] = value.ToString();
            }
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return !context.Values.ContainsKey(Key) 
                ? viewLocations 
                : viewLocations.Select(s => $"/Apps/{context.Values[Key]}" + s).Concat(viewLocations);
        }
    }
}