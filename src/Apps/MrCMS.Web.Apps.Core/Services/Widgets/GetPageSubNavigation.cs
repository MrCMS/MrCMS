using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Helpers;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetPageSubNavigation : IGetPageSubNavigation
    {
        private readonly ISession _session;
        private readonly IGetLiveUrl _getLiveUrl;

        public GetPageSubNavigation(ISession session,
            IGetLiveUrl getLiveUrl)
        {
            _session = session;
            _getLiveUrl = getLiveUrl;
        }

        public async Task<List<NavigationRecord>> GetChildNavigationRecords(Webpage page)
        {
            var webpages = await GetPublishedChildWebpages(page.Id);
            var navigationRecords = new List<NavigationRecord>();
            foreach (var webpage in webpages)
            {
                var publishedChildWebpages = await GetPublishedChildWebpages(webpage.Id);
                var children = new List<NavigationRecord>();
                foreach (var child in publishedChildWebpages)
                {
                    children.Add(new NavigationRecord(child.Name, await _getLiveUrl.GetUrlSegment(child, true),
                        child.Unproxy().GetType()));
                }

                navigationRecords.Add(
                    new NavigationRecord(webpage.Name, await _getLiveUrl.GetUrlSegment(webpage, true),
                        webpage.Unproxy().GetType(), children));
            }

            return navigationRecords;
        }

        private async Task<IList<Webpage>> GetPublishedChildWebpages(int parentId)
        {
            return await _session.QueryOver<Webpage>()
                .Where(
                    webpage =>
                        webpage.Parent.Id == parentId && webpage.RevealInNavigation)
                .Where(webpage => webpage.Published)
                .OrderBy(webpage => webpage.DisplayOrder).Asc
                .Cacheable()
                .ListAsync();
        }
    }
}