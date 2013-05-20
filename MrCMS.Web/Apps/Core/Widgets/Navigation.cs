using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Widgets
{
    public class Navigation : Widget
    {
        public override bool HasProperties
        {
            get { return false; }
        }

        public override object GetModel(ISession session)
        {
            var navigationRecords =
                session.QueryOver<Webpage>().Where(
                    webpage => webpage.Parent == null && webpage.PublishOn != null && webpage.PublishOn <= CurrentRequestData.Now && webpage.RevealInNavigation && webpage.Site == Site).Cacheable()
                       .List().OrderBy(webpage => webpage.DisplayOrder)
                       .Select(webpage => new NavigationRecord
                       {
                           Text = MvcHtmlString.Create(webpage.Name),
                           Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment),
                           Children = webpage.PublishedChildren.Where(webpage1 => webpage1.RevealInNavigation)
                                            .Select(webpage1 =>
                                                    new NavigationRecord
                                                    {
                                                        Text = MvcHtmlString.Create(webpage1.Name),
                                                        Url = MvcHtmlString.Create("/" + webpage1.LiveUrlSegment)
                                                    }).ToList()
                       }).ToList();

            return new NavigationList(navigationRecords.ToList());
        }
    }
}