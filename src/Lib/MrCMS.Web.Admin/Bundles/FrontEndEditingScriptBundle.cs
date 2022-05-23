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
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly SiteSettings _siteSettings;

        public FrontEndEditingScriptBundle(IAccessChecker accessChecker, IGetCurrentUser getCurrentUser,
            SiteSettings siteSettings)
        {
            _accessChecker = accessChecker;
            _getCurrentUser = getCurrentUser;
            _siteSettings = siteSettings;
        }

        public int Priority { get; }

        public async Task<bool> ShouldShow(string theme)
        {
            var user = await _getCurrentUser.Get();
            return await _accessChecker.CanAccess<AdminBarACL>(AdminBarACL.Show, user) &&
                   _siteSettings.EnableInlineEditing;
        }

        public string Url => "/assets/front-end-editing.js";

        public IEnumerable<string> VendorFiles
        {
            get { yield return "/Areas/Admin/Content/lib/ckeditor/ckeditor.js"; }
        }
    }
}