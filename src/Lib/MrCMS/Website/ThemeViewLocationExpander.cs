using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        private const string Key = "theme";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (!context.ActionContext.HttpContext.RequestServices.GetRequiredService<ICheckInstallationStatus>()
                .IsInstalled())
                return;
            var siteSettings = context.ActionContext.HttpContext.RequestServices.GetRequiredService<SiteSettings>();

            if (!string.IsNullOrWhiteSpace(siteSettings?.ThemeName))
                context.Values[Key] = siteSettings.ThemeName;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //throw new System.NotImplementedException();
            foreach (var viewLocation in viewLocations)
            {
                if (context.Values.ContainsKey(Key))
                {
                    yield return $"/Themes/{context.Values[Key]}{viewLocation}";
                }
                yield return viewLocation;
            }
        }
    }
}