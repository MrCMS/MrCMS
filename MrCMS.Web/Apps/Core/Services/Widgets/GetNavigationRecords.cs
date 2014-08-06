using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Widgets;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetNavigationRecords : GetWidgetModelBase<Navigation>
    {
        private readonly ISession _session;

        public GetNavigationRecords(ISession session)
        {
            _session = session;
        }

        public override object GetModel(Navigation widget)
        {
            var navigationRecords =
                GetPages(null).Where(webpage => webpage.Published).OrderBy(webpage => webpage.DisplayOrder)
                    .Select(webpage => new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(webpage.Name),
                        Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment),
                        Children = widget.IncludeChildren ? GetPages(webpage)
                            .Select(webpage1 =>
                                new NavigationRecord
                                {
                                    Text = MvcHtmlString.Create(webpage1.Name),
                                    Url = MvcHtmlString.Create("/" + webpage1.LiveUrlSegment)
                                }).ToList() : null
                    }).ToList();

            return new NavigationList(navigationRecords.ToList());
        }
        private IList<Webpage> GetPages(Webpage parent)
        {
            var queryOver = _session.QueryOver<Webpage>();
            queryOver = parent == null
                ? queryOver.Where(webpage => webpage.Parent == null)
                : queryOver.Where(webpage => webpage.Parent.Id == parent.Id);
            return queryOver.Where(webpage => webpage.RevealInNavigation).Cacheable().List();
        }
    }
}