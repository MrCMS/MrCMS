using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Web.Application.Models;
using NHibernate;

namespace MrCMS.Web.Application.Widgets
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
                    webpage => webpage.Parent == null && webpage.Published && webpage.RevealInNavigation)
                       .OrderBy(webpage => webpage.DisplayOrder).Asc.List()
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