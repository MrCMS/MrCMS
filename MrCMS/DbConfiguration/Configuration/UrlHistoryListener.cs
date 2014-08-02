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
    public class UrlHistoryListener : IPostUpdateEventListener, IOnUpdated
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            //try
            //{
            //    var session = @event.Session.SessionFactory.OpenFilteredSession();
            //    if (@event.Entity is Webpage)
            //    {
            //        var webpage = GetWebpage(session, @event.Entity as Webpage);

            //        var indexOf = @event.Persister.PropertyNames.ToList().IndexOf("UrlSegment");

            //        var oldState = @event.OldState[indexOf];
            //        var newState = @event.State[indexOf];

            //        SaveChangedUrl(oldState, newState, session, webpage);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    CurrentRequestData.ErrorSignal.Raise(exception);
            //}
        }

        private void SaveChangedUrl(object oldState, object newState, ISession session, Webpage webpage)
        {
//check that the URL is different and doesn't already exist in the URL history table.
            if (!StringComparer.OrdinalIgnoreCase.Equals(oldState, newState) && !CheckUrlExistence(session, oldState.ToString()))
            {
                var urlHistory = new UrlHistory
                {
                    Webpage = webpage,
                    UrlSegment = Convert.ToString(oldState),
                    CreatedOn = CurrentRequestData.Now,
                    UpdatedOn = CurrentRequestData.Now,
                    Site = session.Get<Site>(CurrentRequestData.CurrentSite.Id)
                };
                webpage.Urls.Add(urlHistory);
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(urlHistory);
                    transaction.Commit();
                }
            }
        }

        private Webpage GetWebpage(ISession session, Webpage webpage)
        {
            return session.Get<Webpage>(webpage.Id);
        }

        private bool CheckUrlExistence(ISession session, string url)
        {
            return session.QueryOver<UrlHistory>().Where(doc => doc.UrlSegment == url).RowCount() > 0;
        }

        public void Execute(OnUpdatedArgs args)
        {
            var webpage = args.Item as Webpage;
            if (webpage == null)
                return;
            var entityEntry = args.Session.GetSessionImplementation().PersistenceContext.GetEntry(args.Item);
            var loadedState = entityEntry.LoadedState;
            var indexOf = entityEntry.Persister.PropertyNames.ToList().IndexOf("UrlSegment");
            var original = loadedState[indexOf];
            SaveChangedUrl(original, webpage.UrlSegment, args.Session, webpage);
        }
    }
}
