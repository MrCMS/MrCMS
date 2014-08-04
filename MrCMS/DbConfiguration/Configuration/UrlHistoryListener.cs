using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Events;
using MrCMS.Website;
using NHibernate;
using NHibernate.Event;
using System;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.DbConfiguration.Configuration
{
    /// <summary>
    /// Class to auto populate URL History table for webpages. Allows Mr Cms to redirect via 301 response to new resource locations.
    /// </summary>
    public class UrlHistoryListener : IOnUpdated
    {
        private void SaveChangedUrl(string oldUrl, string newUrl, ISession session, Webpage webpage)
        {
            //check that the URL is different and doesn't already exist in the URL history table.
            if (!StringComparer.OrdinalIgnoreCase.Equals(oldUrl, newUrl) && !CheckUrlExistence(session, oldUrl))
            {
                var createdOn = CurrentRequestData.Now;
                var urlHistory = new UrlHistory
                {
                    Webpage = webpage,
                    UrlSegment = Convert.ToString(oldUrl),
                    CreatedOn = createdOn,
                    UpdatedOn = createdOn,
                    Site = session.Get<Site>(CurrentRequestData.CurrentSite.Id)
                };
                webpage.Urls.Add(urlHistory);
                session.Transact(ses => ses.Save(urlHistory));
            }
        }

        private bool CheckUrlExistence(ISession session, string url)
        {
            return session.QueryOver<UrlHistory>().Where(doc => doc.UrlSegment == url).Any();
        }

        public void Execute(OnUpdatedArgs args)
        {
            var webpage = args.Item as Webpage;
            if (webpage == null)
                return;
            var original = args.Original as Webpage;
            string urlSegment = null;
            if (original != null)
                urlSegment = original.UrlSegment;
            SaveChangedUrl(urlSegment, webpage.UrlSegment, args.Session, webpage);
        }
    }
}
