using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class SetTopLevelWebpageDisplayOrder : SetTopLevelDisplayOrder<Webpage>
    {
        public SetTopLevelWebpageDisplayOrder(IGetDocumentsByParent<Webpage> getDocumentsByParent) : base(getDocumentsByParent)
        {
        }
    }
}