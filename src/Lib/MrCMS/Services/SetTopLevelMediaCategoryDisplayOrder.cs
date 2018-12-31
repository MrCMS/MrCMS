using MrCMS.Entities.Documents.Media;

namespace MrCMS.Services
{
    public class SetTopLevelMediaCategoryDisplayOrder : SetTopLevelDisplayOrder<MediaCategory>
    {
        public SetTopLevelMediaCategoryDisplayOrder(IGetDocumentsByParent<MediaCategory> getDocumentsByParent) : base(getDocumentsByParent)
        {
        }
    }
}