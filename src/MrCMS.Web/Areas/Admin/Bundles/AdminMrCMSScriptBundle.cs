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
                yield return "~/Areas/Admin/scripts/menu.js";
                yield return "~/Areas/Admin/scripts/stickyTabs.js";
                yield return "~/Areas/Admin/scripts/admin.js";
                yield return "~/Areas/Admin/scripts/tagging.js";
                yield return "~/Areas/Admin/scripts/search.js";
                yield return "~/Areas/Admin/scripts/batch.js";
                yield return "~/Areas/Admin/scripts/media-uploader.js";
                yield return "~/Areas/Admin/scripts/media-selector.js";
            }
        }
    }
}