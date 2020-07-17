using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Admin.Bundles
{
    public class AdminLibScriptBundle : IScriptBundle
    {
        public string Url
        {
            get { return "~/admin/scripts/lib"; }
        }

        public IEnumerable<string> Files
        {
            get
            {
                yield return "~/Areas/Admin/lib/jquery/jquery-3.3.1.min.js";
                yield return "~/Areas/Admin/lib/jquery/jquery-ui-1.12.1/jquery-ui.js";
                yield return "~/Areas/Admin/lib/jquery/jQuery-Timepicker-Addon-1.6.3/jquery-ui-timepicker-addon.js";
                yield return "~/Areas/Admin/lib/jquery.signalR-2.4.0.js";
                yield return "~/Areas/Admin/lib/store.js";
                yield return "~/Areas/Admin/lib/sweetalert-master/lib/sweet-alert.min.js";
                yield return "~/Areas/Admin/lib/jquery/validate/jquery.validate.js";
                yield return "~/Areas/Admin/lib/jquery/validate/jquery.validate.unobtrusive.js";
                yield return "~/Areas/Admin/lib/jquery/validate/unobtrusive-bootstrap.js";
                yield return "~/Areas/Admin/lib/bootstrap/dist/js/bootstrap.bundle.js";
                yield return "~/Areas/Admin/lib/bootstrap-confirmation.js";
                yield return "~/Areas/Admin/lib/adminlte/js/adminlte.js";
                yield return "~/Areas/Admin/lib/tag-it.min.js";
                yield return "~/Areas/Admin/lib/jquery.noty.packaged.js";
                yield return "~/Areas/Admin/lib/jstree/jstree.js";
                yield return "~/Areas/Admin/lib/dropzone.js";
                yield return "~/Areas/Admin/lib/featherlight.js";
                yield return "~/Areas/Admin/lib/jquery.are-you-sure.js";
                yield return "~/Areas/Admin/lib/update-area.js";
                yield return "~/Areas/Admin/lib/spectrum/spectrum.js";
            }
        }
    }
}