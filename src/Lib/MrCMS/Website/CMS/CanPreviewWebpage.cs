using System.Threading.Tasks;
using MrCMS.ACL.Rules;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.Auth;

namespace MrCMS.Website.CMS
{
    public class CanPreviewWebpage : ICanPreviewWebpage
    {
        private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;
        private readonly IAccessChecker _accessChecker;

        public CanPreviewWebpage(IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal, IAccessChecker accessChecker)
        {
            _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
            _accessChecker = accessChecker;
        }

        public async Task<bool> CanPreview(Webpage webpage)
        {
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            return user != null && await _accessChecker.CanAccess<CoreACL>(CoreACL.CanViewUnpublishedPages, user);
        }
    }
}
