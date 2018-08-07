using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Services
{
    public class SetTopLevelLayoutDisplayOrder : SetTopLevelDisplayOrder<Layout>
    {
        public SetTopLevelLayoutDisplayOrder(IGetDocumentsByParent<Layout> getDocumentsByParent) : base(getDocumentsByParent)
        {
        }
    }
}