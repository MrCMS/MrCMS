using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;

namespace MrCMS.Website
{
    public class AppViewLocationExpander : IViewLocationExpander
    {
        public const string Key = "app";

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
            foreach (var viewLocation in viewLocations)
            {
                if (context.Values.ContainsKey(Key))
                {
                    yield return $"/Apps/{context.Values[Key]}{viewLocation}";
                }

                yield return viewLocation;
            }
        }
    }
}