using System.Threading.Tasks;
using MrCMS.ACL.Rules;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Auth
{
    public class FrontEndEditingChecker : IFrontEndEditingChecker
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IAccessChecker _accessChecker;
        private readonly IConfigurationProvider _configurationProvider;

        public FrontEndEditingChecker(IGetCurrentUser getCurrentUser, IAccessChecker accessChecker, IConfigurationProvider configurationProvider)
        {
            _getCurrentUser = getCurrentUser;
            _accessChecker = accessChecker;
            _configurationProvider = configurationProvider;
        }

        public async Task<bool> IsAllowed()
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            if (!siteSettings.EnableInlineEditing)
                return false;
            var user = _getCurrentUser.Get();
            return user != null && await _accessChecker.CanAccess<AdminBarACL>(AdminBarACL.Show, user);
        }
    }
}