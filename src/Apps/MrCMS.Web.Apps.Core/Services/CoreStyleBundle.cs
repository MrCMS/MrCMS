using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Core.Services
{
    public class CoreStyleBundle : IUIStyleBundle
    {
        public int Priority { get; }
        public Task<bool> ShouldShow(string theme) => Task.FromResult(string.IsNullOrWhiteSpace(theme));

        public string Url => "/assets/core.css";
        public string MinifiedUrl => "/assets/core.min.css";

        public IEnumerable<string> VendorFiles
        {
            get { yield return "/Content/lib/bootstrap/dist/css/bootstrap.min.css"; }
        }
    }
}