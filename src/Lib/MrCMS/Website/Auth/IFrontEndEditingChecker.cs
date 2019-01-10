using MrCMS.ACL.Rules;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Auth
{
    public interface IFrontEndEditingChecker
    {
        bool IsAllowed();
    }

    public class FrontEndEditingChecker : IFrontEndEditingChecker
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IAccessChecker _accessChecker;
        private readonly SiteSettings _siteSettings;

        public FrontEndEditingChecker(IGetCurrentUser getCurrentUser, IAccessChecker accessChecker, SiteSettings siteSettings)
        {
            _getCurrentUser = getCurrentUser;
            _accessChecker = accessChecker;
            _siteSettings = siteSettings;
        }

        public bool IsAllowed()
        {
            if (!_siteSettings.EnableInlineEditing)
                return false;
            var user = _getCurrentUser.Get();
            return user != null && _accessChecker.CanAccess<AdminBarACL>(AdminBarACL.Show, user);
        }
    }
}