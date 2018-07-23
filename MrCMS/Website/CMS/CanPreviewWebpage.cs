using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class CanPreviewWebpage : ICanPreviewWebpage
    {
        private readonly IGetCurrentUser _getCurrentUser;
        //private readonly IUserRoleManager _userRoleManager;

        public CanPreviewWebpage(IGetCurrentUser getCurrentUser)//, IUserRoleManager userRoleManager)
        {
            _getCurrentUser = getCurrentUser;
            //_userRoleManager = userRoleManager;
        }

        public async Task<bool> CanPreview(Webpage webpage)
        {
            var user = _getCurrentUser.Get();
            if (user == null)
                return false;

            return true;
            // TODO: role check
            //return await _userRoleManager.IsInRoleAsync(user, UserRole.Administrator);
        }
    }
}