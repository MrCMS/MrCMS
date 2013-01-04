using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface INavigationService
    {
        SiteTree<Webpage> GetWebsiteTree(Site site, int? depth = null);
        SiteTree<MediaCategory> GetMediaTree();
        SiteTree<Layout> GetLayoutList(Site site);
        SiteTree<User> GetUserList();
        IEnumerable<SelectListItem> GetParentsList(Site site);
        string GetSiteMap(UrlHelper urlHelper, Site site);
        IEnumerable<SelectListItem> GetDocumentTypes(string type);
    }

    public class GetMoreResult : List<SiteTreeNode>
    {
    }
}