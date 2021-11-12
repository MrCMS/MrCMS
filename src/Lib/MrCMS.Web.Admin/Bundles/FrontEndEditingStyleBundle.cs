using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.ACL.Rules;
using MrCMS.Settings;
using MrCMS.Website.Auth;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Admin.Bundles
{
    public class FrontEndEditingStyleBundle : IUIStyleBundle
    {
        private readonly IAccessChecker _accessChecker;
        private readonly SiteSettings _siteSettings;

        public FrontEndEditingStyleBundle(IAccessChecker accessChecker, SiteSettings siteSettings)
        {
            _accessChecker = accessChecker;
            _siteSettings = siteSettings;
        }

        public int Priority { get; }

        public async Task<bool> ShouldShow(string theme)
        {
            return await _accessChecker.CanAccess<AdminBarACL>("Show") && _siteSettings.EnableInlineEditing;
        }

        public string Url => "/assets/front-end-editing.css";
        public string MinifiedUrl => "/assets/front-end-editing.min.css";

        public IEnumerable<string> VendorFiles
        {
            get
            {
                yield return "https://cdnjs.cloudflare.com/ajax/libs/featherlight/1.7.13/featherlight.min.css";
                // yield return "/Areas/Admin/Content/lib/ckeditor/ckeditor.js";
                // yield return "/Areas/Admin/Content/lib/jquery/jquery-ui-1.12.1/jquery-ui.js";
            }
        }
    }
}