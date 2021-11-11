using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Core.Services
{
    public class CoreVendorScriptBundle : IUIScriptBundle
    {
        public int Priority { get; }
        public Task<bool> ShouldShow(string theme) => Task.FromResult(string.IsNullOrWhiteSpace(theme));

        public string Url { get; }
        public string MinifiedUrl { get; }

        public IEnumerable<string> VendorFiles
        {
            get
            {
                yield return "/assets/lib/jquery/jquery-3.3.1.min.js";
                yield return "/assets/lib/jquery/validate/jquery.validate.js";
                yield return "/assets/lib/jquery/validate/jquery.validate.unobtrusive.js";
                yield return "/assets/lib/jquery/validate/additional-methods.min.js";
                yield return "/assets/lib/bootstrap/dist/js/bootstrap.bundle.js";
            }
        }
    }
}