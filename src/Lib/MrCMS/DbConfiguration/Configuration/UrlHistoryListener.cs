using System;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Events;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.DbConfiguration.Configuration
{
    /// <summary>
    ///     Class to auto populate URL History table for webpages. Allows Mr Cms to redirect via 301 response to new resource
    ///     locations.
    /// </summary>
    public class UrlHistoryListener : IOnUpdated<Webpage>
    {
        public async Task Execute(OnUpdatedArgs<Webpage> args)
        {
            Webpage webpage = args.Item;
            if (webpage == null)
                return;
            Webpage original = args.Original;
            string urlSegment = null;
            if (original != null)
                urlSegment = original.UrlSegment;
            await SaveChangedUrl(urlSegment, webpage.UrlSegment, args.Session, webpage);
        }

        private async Task SaveChangedUrl(string oldUrl, string newUrl, ISession session, Webpage webpage)
        {
            //check that the URL is different and doesn't already exist in the URL history table.
            if (!StringComparer.OrdinalIgnoreCase.Equals(oldUrl, newUrl))
            {
                var existingUrlHistory = await GetExistingUrlHistory(session, oldUrl);
                if (existingUrlHistory == null)
                {
                    DateTime createdOn = DateTime.UtcNow;
                    var urlHistory = new UrlHistory
                    {
                        Webpage = webpage,
                        UrlSegment = Convert.ToString(oldUrl),
                        CreatedOn = createdOn,
                        UpdatedOn = createdOn,
                        Site = session.Get<Site>(webpage.Site.Id)
                    };
                    webpage.Urls.Add(urlHistory);
                    await session.TransactAsync((ses, token) => ses.SaveAsync(urlHistory, token));
                }
                else if (existingUrlHistory.Webpage == null)
                {
                    DateTime updatedOn = DateTime.UtcNow;
                    existingUrlHistory.Webpage = webpage;
                    existingUrlHistory.UpdatedOn = updatedOn;
                    webpage.Urls.Add(existingUrlHistory);
                    await session.TransactAsync((ses, token) => ses.UpdateAsync(existingUrlHistory, token));
                }
            }
        }

        private async Task<UrlHistory> GetExistingUrlHistory(ISession session, string url)
        {
            return await session.Query<UrlHistory>().FirstOrDefaultAsync(doc => doc.UrlSegment == url);
        }
    }
}