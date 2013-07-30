using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

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
    public class DetailedSearchResultModel : SearchResultModel
    {
        public string CreatedOn { get; set; }
        public string EditUrl { get; set; }
        public string ViewUrl { get; set; }
        public string AddChildUrl { get; set; }
        public bool HasChildren { get; set; }
        public Webpage Webpage { get; set; }
    }
}