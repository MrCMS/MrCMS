using MrCMS.Data;

namespace MrCMS.Entities.Documents
{
    public class DocumentTag : IJoinTable
    {
        public Document Document { get; set; }
        public int DocumentId { get; set; }
        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}