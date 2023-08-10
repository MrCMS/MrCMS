using System.Threading.Tasks;
using MrCMS.ACL.Rules;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.Auth;

namespace MrCMS.Website.CMS
{
    public class CanPreviewWebpage : ICanPreviewWebpage
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IAccessChecker _accessChecker;

        public CanPreviewWebpage(IGetCurrentUser getCurrentUser, IAccessChecker accessChecker)
        {
            _getCurrentUser = getCurrentUser;
            _accessChecker = accessChecker;
        }

        public async Task<bool> CanPreview(Webpage webpage)
        {
            var user = await _getCurrentUser.Get();
            return user != null && await _accessChecker.CanAccess<CoreACL>(CoreACL.CanViewUnpublishedPages, user);
        }
    }
}
