using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Core.Bundles
{
    public class CoreScriptBundle : IUIScriptBundle
    {
        public int Priority { get; }
        public Task<bool> ShouldShow(string theme) => Task.FromResult(true);

        public string Url => "/Apps/Core/assets/core-front-end.js";

        public IEnumerable<string> VendorFiles
        {
            get
            {
                yield break;
            }
        }
    }
}