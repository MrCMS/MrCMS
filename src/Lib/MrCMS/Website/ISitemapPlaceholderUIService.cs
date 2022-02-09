using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface ISitemapPlaceholderUIService
    {
        Task<RedirectResult> Redirect(SitemapPlaceholder page);
    }
}