using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Admin.Bundles
{
    public class AdminMrCMSStylesheetBundle : IAdminStyleBundle
    {
        public int Priority => int.MaxValue;

        public Task<bool> ShouldShow(string theme)
        {
            return Task.FromResult(true);
        }

        public string Url => "/Areas/Admin/assets/admin.css";

        public IEnumerable<string> VendorFiles
        {
            get
            {
                yield return "/Areas/Admin/Content/lib/jquery-ui-bootstrap/jquery-ui-1.9.2.custom.css";
                yield return "/Areas/Admin/Content/lib/font-awesome/css/font-awesome.css";
                //yield return "/Areas/Admin/Content/lib/sweetalert-master/lib/sweet-alert.css";
                yield return "/Areas/Admin/Content/lib/select2-4.0.13/css/select2.css";
                yield return "/Areas/Admin/Content/lib/select2-bootstrap4-theme-1.5.2/select2-bootstrap4.css";
                yield return "/Areas/Admin/Content/lib/spectrum/spectrum.css";
                //yield return "https://cdnjs.cloudflare.com/ajax/libs/featherlight/1.7.13/featherlight.min.css";
            }
        }
    }
}
