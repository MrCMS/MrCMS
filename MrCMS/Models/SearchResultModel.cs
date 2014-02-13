using MrCMS.Entities.Documents;

namespace MrCMS.Models
{
    public class SearchResultModel
    {
        public string Name { get; set; }
        public string DocumentId { get; set; }
        public string DocumentType { get; set; }
        public string LastUpdated { get; set; }
        public string DisplayName { get; set; }
    }
}