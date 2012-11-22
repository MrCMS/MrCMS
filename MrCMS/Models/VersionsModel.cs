using MrCMS.Entities.Documents;
using MrCMS.Paging;

namespace MrCMS.Models
{
    public class VersionsModel : AsyncListModel<DocumentVersion>
    {
        public VersionsModel(PagedList<DocumentVersion> items, int id)
            : base(items, id)
        {
        }
    }
}