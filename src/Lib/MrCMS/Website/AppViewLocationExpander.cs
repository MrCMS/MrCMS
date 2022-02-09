using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;

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

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            foreach (var viewLocation in viewLocations)
            {
                if (context.Values.ContainsKey(Key))
                {
                    var appName = context.Values[Key];
                    yield return $"/Apps/{appName}{viewLocation}";
                }

                yield return viewLocation;

                foreach (var appName in context.ActionContext.HttpContext.RequestServices
                    .GetRequiredService<MrCMSAppContext>().Apps.Select(x => x.Name))
                {
                    yield return $"/Apps/{appName}{viewLocation}";
                }
            }
        }
    }
}