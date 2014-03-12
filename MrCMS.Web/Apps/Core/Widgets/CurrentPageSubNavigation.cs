using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Widgets
{
    public class CurrentPageSubNavigation : Widget
    {
        [DisplayName("Show Name As Title")]
        public virtual bool ShowNameAsTitle { get; set; }

        public override object GetModel(ISession session)
        {
            var webpages =
                GetPublishedChildWebpages(session, CurrentRequestData.CurrentPage.Id);
            var navigationRecords =
                webpages
                    .Select(webpage => new NavigationRecord
                        {
                            Text = MvcHtmlString.Create(webpage.Name),
                            Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment),
                            Children = GetPublishedChildWebpages(session, webpage.Id)
                                           .Select(webpage1 =>
                                                   new NavigationRecord
                                                       {
                                                           Text = MvcHtmlString.Create(webpage1.Name),
                                                           Url = MvcHtmlString.Create("/" + webpage1.LiveUrlSegment)
                                                       }).ToList()
                        }).ToList();

            return new CurrentPageSubNavigationModel
                       {
                           NavigationList = navigationRecords.ToList(),
                           Model = this
                       }; 
        }

        private static IEnumerable<Webpage> GetPublishedChildWebpages(ISession session, int parentId)
        {
            return session.QueryOver<Webpage>()
                          .Where(
                              webpage =>
                              webpage.Parent.Id == parentId && webpage.RevealInNavigation)
                          .OrderBy(webpage => webpage.DisplayOrder).Asc
                          .Cacheable()
                          .List()
                          .Where(webpage => webpage.Published);
        }
    }

    public class CurrentPageSubNavigationModel
    {
        public List<NavigationRecord> NavigationList { get; set; }
        public CurrentPageSubNavigation Model { get; set; }
    }
}