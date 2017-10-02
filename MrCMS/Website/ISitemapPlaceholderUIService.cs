using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface ISitemapPlaceholderUIService
    {
        RedirectResult Redirect(SitemapPlaceholder page);
    }
}