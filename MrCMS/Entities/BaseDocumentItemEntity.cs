using MrCMS.Entities.Documents;

namespace MrCMS.Entities
{
    public abstract class BaseDocumentItemEntity : BaseEntity
    {
        public virtual Document Document { get; set; }
    }
}
