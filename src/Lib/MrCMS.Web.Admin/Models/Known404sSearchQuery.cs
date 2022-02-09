using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Models
{
    public class Known404sSearchQuery : IRedirectsSearchQuery
    {
        public Known404sSearchQuery()
        {
            Page = 1;
        }

        public int Page { get; set; }
        public string Url { get; set; }
        public UrlHistoryType? Type => UrlHistoryType.Unhandled;
        public RedirectSortBy SortBy => RedirectSortBy.FailedLookupCount;
    }
}