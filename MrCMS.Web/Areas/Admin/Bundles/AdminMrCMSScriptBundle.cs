using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Areas.Admin.Bundles
{
    public class AdminMrCMSScriptBundle : IScriptBundle
    {
        public string Url
        {
            get { return "~/admin/scripts/mrcms"; }
        }

        public IEnumerable<string> Files
        {
            get
            {
                yield return "~/Areas/Admin/Content/Scripts/mrcms/menu.js";
                yield return "~/Areas/Admin/Content/Scripts/mrcms/stickyTabs.js";
                yield return "~/Areas/Admin/Content/Scripts/mrcms/admin.js";
                yield return "~/Areas/Admin/Content/Scripts/mrcms/tagging.js";
                yield return "~/Areas/Admin/Content/Scripts/mrcms/search.js";
                yield return "~/Areas/Admin/Content/Scripts/mrcms/batch.js";
                yield return "~/Areas/Admin/Content/Scripts/mrcms/media-uploader.js";
                yield return "~/Areas/Admin/Content/Scripts/mrcms/mrcms-media-selector.js";
            }
        }
    }
}