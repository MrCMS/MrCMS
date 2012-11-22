using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.AddOns.Pages.Portfolio;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class RelatedPortfolio : Widget
    {
        private int _count=4;

        public virtual int Count { get { return _count; } set { _count = value; } }
        public override object GetModel(NHibernate.ISession session)
        {
            var page = MrCMSApplication.CurrentPage as PortfolioItem;

            if (page == null)
                return null;

            var tag = page.Tags.FirstOrDefault(x=>page.Container.Tags.Contains(x));

            if (tag == null)
                return null;

            var relatedItems = page.Container.ChildItems.Where(x => x.Tags.Contains(tag) && x.UrlSegment != page.UrlSegment).Take(Count);

            return relatedItems;
        }
    }
}
