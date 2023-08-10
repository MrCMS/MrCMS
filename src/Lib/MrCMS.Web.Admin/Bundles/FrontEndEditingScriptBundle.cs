using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.ACL.Rules;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Auth;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Admin.Bundles
{
    public class FrontEndEditingScriptBundle : IUIScriptBundle
    {
        private readonly IAccessChecker _accessChecker;
        private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;
        private readonly SiteSettings _siteSettings;

        public FrontEndEditingScriptBundle(IAccessChecker accessChecker,
            IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal,
            SiteSettings siteSettings)
        {
            _accessChecker = accessChecker;
            _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
            _siteSettings = siteSettings;
        }

        public int Priority => int.MaxValue;

        public async Task<bool> ShouldShow(string theme)
        {
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            return await _accessChecker.CanAccess<AdminBarACL>(AdminBarACL.Show, user) &&
                   _siteSettings.EnableInlineEditing;
        }

        public string Url => "/Areas/Admin/assets/front-end-editing.js";

        public IEnumerable<string> VendorFiles
        {
            get { yield return "/Areas/Admin/Content/lib/ckeditor/ckeditor.js"; }
        }
    }
}