using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Core.Bundles
{
    public class CoreStyleBundle : IUIStyleBundle
    {
        public int Priority { get; }
        public Task<bool> ShouldShow(string theme) => Task.FromResult(string.IsNullOrWhiteSpace(theme));

        public string Url => "/Apps/Core/assets/core-front-end.css";

        public IEnumerable<string> VendorFiles
        {
            get
            {
                yield break;
            }
        }
    }
}