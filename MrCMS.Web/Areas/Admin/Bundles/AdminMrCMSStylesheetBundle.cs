using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Areas.Admin.Bundles
{
    public class AdminMrCMSStylesheetBundle : IStylesheetBundle
    {
        public string Url { get { return "~/admin/stylesheets/mrcms"; } }

        public IEnumerable<string> Files
        {
            get
            {
                yield return "~/Areas/Admin/Content/Styles/jquery.mrcms-mediaselector.css";
                yield return "~/Areas/Admin/Content/Styles/featherlight.css";
                yield return "~/Areas/Admin/Content/Styles/jquery.tagit.css";
                yield return "~/Areas/Admin/Content/Styles/jquery-ui-bootstrap/jquery-ui-1.9.2.custom.css";
                yield return "~/Areas/Admin/Content/plugins/font-awesome-4.2.0/css/font-awesome.css";
                yield return "~/Areas/Admin/Content/Scripts/lib/jstree/themes/default/style.css";
                yield return "~/Areas/Admin/Content/Styles/bootstrap/css/bootstrap.css";
                yield return "~/Areas/Admin/Content/Styles/bootstrap/css/bootstrap-theme.css";
                yield return "~/Areas/Admin/Content/plugins/sweetalert-master/lib/sweet-alert.css";
                yield return "~/Areas/Admin/Content/Styles/dropzone.css";
                yield return "~/Areas/Admin/Content/Styles/mrcms-admin.css";
            }
        }
    }
}