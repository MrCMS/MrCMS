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

        public bool CanPreview(Webpage webpage)
        {
            var user = _getCurrentUser.Get();
            if (user == null)
                return false;

            return user.IsAdmin;
        }
    }
}