using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface ITagPageUIService
    {
        IPagedList<Webpage> GetWebpages(TagPage page, TagPageSearchModel model);
        TagPage GetPage(in int id);
    }
}