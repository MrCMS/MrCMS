using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.DbConfiguration.Configuration
{
    /// <summary>
    ///     Class to auto populate URL History table for webpages. Allows Mr Cms to redirect via 301 response to new resource
    ///     locations.
    /// </summary>
    public class UrlHistoryListener : IOnUpdated<Webpage>
    {
        public void Execute(OnUpdatedArgs<Webpage> args)
        {
            Webpage webpage = args.Item;
            if (webpage == null)
                return;
            Webpage original = args.Original;
            string urlSegment = null;
            if (original != null)
                urlSegment = original.UrlSegment;
            SaveChangedUrl(urlSegment, webpage.UrlSegment, args.Session, webpage);
        }

        private void SaveChangedUrl(string oldUrl, string newUrl, ISession session, Webpage webpage)
        {
            //check that the URL is different and doesn't already exist in the URL history table.
            if (!StringComparer.OrdinalIgnoreCase.Equals(oldUrl, newUrl) && !CheckUrlExistence(session, oldUrl))
            {
                DateTime createdOn = CurrentRequestData.Now;
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
    }
}