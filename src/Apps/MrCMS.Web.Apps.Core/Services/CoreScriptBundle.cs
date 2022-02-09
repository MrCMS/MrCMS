using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Core.Services
{
    public class CoreScriptBundle : IUIScriptBundle
    {
        public int Priority { get; }
        public Task<bool> ShouldShow(string theme) => Task.FromResult(true);

        public string Url => "/Apps/Core/assets/core.js";

        public IEnumerable<string> VendorFiles
        {
            get { yield break; }
        }
    }
}