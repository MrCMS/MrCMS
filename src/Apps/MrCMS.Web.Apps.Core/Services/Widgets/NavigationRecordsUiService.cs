using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Models.Navigation;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class NavigationRecordsUiService : INavigationRecordsUiService
    {
        private readonly ISession _session;

        public NavigationRecordsUiService(ISession session)
        {
            _session = session;
        }

        public NavigationList GetNavigationList()
        {
            var rootPages = GetPages(null);
            var childPages = GetPages(rootPages);
            var navigationRecords =
                rootPages
                    .Select(webpage => new NavigationRecord
                    (webpage.Name, webpage.Url,
                        webpage.GetType(), childPages.Where(y => y.ParentId == webpage.Id)
                            .Select(child =>
                                new NavigationRecord(child.Name, child.Url,
                                    child.PageType)
                            ).ToList()));

            var navigationList = new NavigationList(navigationRecords.ToList());
            return navigationList;
        }

        private IList<NavigationDto> GetPages(IList<NavigationDto> parents)
        {
            var queryOver = _session.Query<Webpage>();
            if (parents == null)
            {
                queryOver = queryOver.Where(webpage => webpage.Parent == null);
            }
            else
            {
                var parentIds = parents.Select(p => p.Id).ToList();
                queryOver = queryOver.Where(webpage => parentIds.Contains(webpage.Parent.Id));
            }

            queryOver = queryOver.Where(webpage => webpage.RevealInNavigation && webpage.Published)
                .OrderBy(webpage => webpage.DisplayOrder);
            ;

            return
                queryOver.Select(x => new NavigationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.Parent != null ? x.Parent.Id : null,
                    Url = x.UrlSegment.GetRelativeUrl(),
                    PageType = x.GetType()
                }).WithOptions(options => options.SetCacheable(true)).ToList();
        }

        private class NavigationDto
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public int? ParentId { get; init; }
            public string Url { get; init; }
            public Type PageType { get; init; }
        }
    }
}