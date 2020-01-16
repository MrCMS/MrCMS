using MrCMS.Data;

namespace MrCMS.Entities.Documents
{
    public class DocumentTag : IJoinTable
    {
        public virtual Document Document { get; set; }
        public int DocumentId { get; set; }
        public virtual Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}