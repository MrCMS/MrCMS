using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Models
{
    public class RedirectsSearchQuery : IRedirectsSearchQuery
    {
        public RedirectsSearchQuery()
        {
            Page = 1;
            SortBy = RedirectSortBy.FailedLookupCount;
        }

        public int Page { get; set; }
        [Display(Name = "Url")] public string Url { get; set; }
        public UrlHistoryType? Type { get; set; }
        public RedirectSortBy SortBy { get; set; }
    }
}