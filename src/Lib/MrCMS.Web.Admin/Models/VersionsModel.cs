using X.PagedList;

namespace MrCMS.Web.Admin.Models
{
    public class VersionsModel : AsyncListModel<WebpageVersionViewModel>
    {
        public VersionsModel(IPagedList<WebpageVersionViewModel> items, int id)
            : base(items, id)
        {
        }
    }
}