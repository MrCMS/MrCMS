using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class CanPreviewWebpage : ICanPreviewWebpage
    {
        private readonly IGetCurrentUser _getCurrentUser;

        public CanPreviewWebpage(IGetCurrentUser getCurrentUser)
        {
            _getCurrentUser = getCurrentUser;
        }

        public async Task<bool> CanPreview(Webpage webpage)
        {
            var user = await _getCurrentUser.Get();
            return user != null && user.IsAdmin;
        }
    }
}