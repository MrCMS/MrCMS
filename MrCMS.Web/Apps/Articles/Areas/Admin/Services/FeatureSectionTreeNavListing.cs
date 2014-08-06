using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class FeatureSectionTreeNavListing : WebpageTreeNavListing<FeatureSection>
    {
        private readonly ISession _session;
        private readonly UrlHelper _urlHelper;

        public FeatureSectionTreeNavListing(ISession session, UrlHelper urlHelper)
        {
            _session = session;
            _urlHelper = urlHelper;
        }

        public override AdminTree GetTree(int? id)
        {
            var parent = _session.Get<Webpage>(id);
            var adminTree = new AdminTree { RootContoller = "Webpage" };
            var query = _session.QueryOver<FeatureSection>().Where(x => x.Parent.Id == id);
            if (id.HasValue)
            {
                if (parent != null)
                {
                    query = query.OrderBy(webpage => webpage.DisplayOrder).Asc;
                }
            }
            else
            {
                adminTree.IsRootRequest = true;
                query = query.OrderBy(x => x.DisplayOrder).Asc;
            }

            query.Cacheable().List().ForEach(doc =>
            {
                var documentMetadata = doc.GetMetadata();
                var node = new AdminTreeNode
                {
                    Id = doc.Id,
                    ParentId = doc.ParentId,
                    Name = doc.Name,
                    IconClass = documentMetadata.IconClass,
                    NodeType = "Webpage",
                    Type = documentMetadata.Type.FullName,
                    HasChildren = _session.QueryOver<FeatureSection>().Where(webpage => webpage.Parent.Id == doc.Id).Cacheable().Any(),
                    Sortable = documentMetadata.Sortable,
                    CanAddChild = true,
                    IsPublished = doc.Published,
                    RevealInNavigation = doc.RevealInNavigation,
                    Url = _urlHelper.Action("Edit", "Webpage", new { id = doc.Id })
                };
                adminTree.Nodes.Add(node);
            });
            return adminTree;
        }
    }
}