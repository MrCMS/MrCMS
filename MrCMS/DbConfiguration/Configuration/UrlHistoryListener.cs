using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
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
    public class UrlHistoryListener : IPostUpdateEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            try
            {
                var session = @event.Session.SessionFactory.OpenFilteredSession();
                if (@event.Entity is Webpage)
                {
                    var webpage = GetWebpage(session, @event.Entity as Webpage);

                    var indexOf = @event.Persister.PropertyNames.ToList().IndexOf("UrlSegment");

                    var oldState = @event.OldState[indexOf];
                    var newState = @event.State[indexOf];

                    //check that the URL is different and doesn't already exist in the URL history table.
                    if (!StringComparer.OrdinalIgnoreCase.Equals(oldState,newState) && !CheckUrlExistence(session, oldState.ToString()))
                    {
                        var urlHistory = new UrlHistory
                        {
                            Webpage = webpage,
                            UrlSegment = Convert.ToString(@event.OldState[indexOf]),
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
            }
            catch (Exception exception)
            {
                CurrentRequestData.ErrorSignal.Raise(exception);
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
    }
}
