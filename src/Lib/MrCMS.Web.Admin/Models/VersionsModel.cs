using X.PagedList;

namespace MrCMS.Web.Admin.Models
{
    public class VersionsModel : AsyncListModel<DocumentVersionViewModel>
    {
        public VersionsModel(IPagedList<DocumentVersionViewModel> items, int id)
            : base(items, id)
        {
        }
    }
}