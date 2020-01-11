using MrCMS.Entities.People;

namespace MrCMS.Entities.Documents
{
    public class DocumentVersion : SiteEntity
    {
        public virtual Document Document { get; set; }
        public int DocumentId { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual string Data { get; set; }
    }
}