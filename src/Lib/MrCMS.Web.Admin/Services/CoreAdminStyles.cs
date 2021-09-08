using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Admin.Services
{
    public class CoreAdminStyles : IAdminStyleBundle
    {
        public int Priority { get; }

        public Task<bool> ShouldShow(string theme)
        {
            return Task.FromResult(true);
        }

        public string Url => "/Areas/Admin/Content/admin.css";
        public string MinifiedUrl => "/Areas/Admin/Content/admin.min.css";

        public IEnumerable<string> VendorFiles
        {
            get { yield break; }
        }
    }
}