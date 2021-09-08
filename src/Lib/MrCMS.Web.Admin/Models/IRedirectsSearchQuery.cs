using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Models
{
    public interface IRedirectsSearchQuery
    {
        int Page { get; }
        string Url { get; }
        UrlHistoryType? Type { get; }
        RedirectSortBy SortBy { get; }
    }
}