using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface INavigationService
    {
        SiteTree<Webpage> GetWebsiteTree(int? depth = null);
        SiteTree<MediaCategory> GetMediaTree();
        SiteTree<Layout> GetLayoutList();
        SiteTree<User> GetUserList();
        GetMoreResult GetMore(int parentId, int previousId);
        IEnumerable<SelectListItem> GetParentsList();
        string GetSiteMap(UrlHelper urlHelper);
    }

    public class GetMoreResult : List<SiteTreeNode>
    {
    }
}