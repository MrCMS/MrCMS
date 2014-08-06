using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class VersionsModel : AsyncListModel<DocumentVersion>
    {
        public VersionsModel(IPagedList<DocumentVersion> items, int id)
            : base(items, id)
        {
        }
    }
}