using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Admin.Bundles
{
    public class AdminMrCMSStylesheetBundle : IStylesheetBundle
    {
        public string Url { get { return "~/admin/stylesheets/mrcms"; } }

        public IEnumerable<string> Files
        {
            get
            {
                yield return "~/Areas/Admin/styles/jquery.mrcms-mediaselector.css";
                yield return "~/Areas/Admin/styles/featherlight.css";
                yield return "~/Areas/Admin/styles/jquery.tagit.css";
                yield return "~/Areas/Admin/styles/jquery-ui-bootstrap/jquery-ui-1.9.2.custom.css";
                yield return "~/Areas/Admin/lib/font-awesome/css/font-awesome.css";
                yield return "~/Areas/Admin/lib/jstree/themes/default/style.css";
                yield return "~/Areas/Admin/lib/adminlte/css/adminlte.css";
                
                //yield return "~/Areas/Admin/lib/bootstrap/dist/css/bootstrap.css";
                yield return "~/Areas/Admin/lib/sweetalert-master/lib/sweet-alert.css";
                yield return "~/Areas/Admin/styles/dropzone.css";
                yield return "~/Areas/Admin/styles/mrcms-admin.css";
                yield return "~/Areas/Admin/lib/spectrum/spectrum.css";
            }
        }
    }
}