using System.Web.Mvc;
using System.Web.Optimization;
using MrCMS.Apps;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("batch execute", "batch-run/next/{id}",
                new { controller = "BatchExecution", action = "ExecuteNext" });

            context.MapRoute(
                "Admin_default1",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );

            BundleTable.Bundles.Add(GenerateAdminLibBundel());
            BundleTable.Bundles.Add(GenerateMrCmsAdminLibBundle());
        }

        private Bundle GenerateMrCmsAdminLibBundle()
        {
            var adminMrCmsBundle = new Bundle("~/admin/scripts/mrcms");

            adminMrCmsBundle.Include("~/Areas/Admin/Content/Scripts/mrcms/menu.js");
            adminMrCmsBundle.Include("~/Areas/Admin/Content/Scripts/mrcms/stickyTabs.js");
            adminMrCmsBundle.Include("~/Areas/Admin/Content/Scripts/mrcms/admin.js");
            adminMrCmsBundle.Include("~/Areas/Admin/Content/Scripts/mrcms/tagging.js");
            adminMrCmsBundle.Include("~/Areas/Admin/Content/Scripts/mrcms/search.js");
            adminMrCmsBundle.Include("~/Areas/Admin/Content/Scripts/mrcms/batch.js");
            adminMrCmsBundle.Include("~/Areas/Admin/Content/Scripts/mrcms/media-uploader.js");
            adminMrCmsBundle.Include("~/Areas/Admin/Content/Scripts/mrcms/mrcms-media-selector.js");

            return adminMrCmsBundle;
        }

        public Bundle GenerateAdminLibBundel()
        {
            var adminBundle = new Bundle("~/admin/scripts/lib");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery/jquery-1.11.2.min.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery/ui/jquery-ui.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery/ui/jquery-ui-timepicker-addon.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery/ui/i18n/jquery.ui.datepicker-" + CurrentRequestData.CultureInfo.Name + ".js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery.signalR-2.2.0.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/store.js");
            adminBundle.Include("~/Areas/Admin/Content/plugins/sweetalert-master/lib/sweet-alert.min.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery/validate/jquery.validate.js");
            if (!CurrentRequestData.CultureInfo.Name.StartsWith("en"))
            {
                adminBundle.Include(string.Format("~/Areas/Admin/Content/Scripts/lib/jquery/validate/localization/messages_{0}.js", CurrentRequestData.CultureInfo.Name.Replace("-", "_")));
            }
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery/validate/jquery.validate.unobtrusive.min.js");
            adminBundle.Include("~/Areas/Admin/Content/Styles/bootstrap/js/bootstrap.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/tag-it.min.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery.noty.packaged.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jstree/jstree.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/dropzone.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/featherlight.js");
            adminBundle.Include("~/Areas/Admin/Content/Scripts/lib/jquery.are-you-sure.js");

            return adminBundle;
        }
    }
}