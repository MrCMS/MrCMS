using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Core.Bundles
{
    public class CoreJqueryBundle : IUIScriptBundle
    {
        public int Priority { get; }
        public Task<bool> ShouldShow(string theme) => Task.FromResult(string.IsNullOrWhiteSpace(theme));

        public string Url => "/Apps/Core/assets/core-jquery.js";

        public IEnumerable<string> VendorFiles
        {
            get
            {
                 yield return "/Apps/Core/Content/lib/jquery/jquery-3.6.0.min.js";
                 yield return "/Apps/Core/Content/lib/jquery/validate/jquery.validate.min.js";
                 yield return "/Apps/Core/Content/lib/jquery/validate/jquery.validate.unobtrusive.js";
            }
        }
    }
}