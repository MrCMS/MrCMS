using MrCMS.Entities.People;

namespace MrCMS.Entities.Documents
{
    public class DocumentVersion : SiteEntity
    {
        public Document Document { get; set; }
        public int DocumentId { get; set; }
        public User User { get; set; }
        public int? UserId { get; set; }
        public string Data { get; set; }
    }
}