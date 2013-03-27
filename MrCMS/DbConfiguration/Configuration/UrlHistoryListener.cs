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
    public class UrlHistoryListener : IPostUpdateEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            try
            {
                var session = @event.Session.SessionFactory.OpenSession();
                if (@event.Entity is Webpage)
                {
                    var webpage = GetWebpage(session, @event.Entity as Webpage);

                    var indexOf = @event.Persister.PropertyNames.ToList().IndexOf("UrlSegment");

                    if (@event.OldState[indexOf] != @event.State[indexOf] && webpage.UrlSegment != Convert.ToString(@event.State[indexOf]) && !CheckUrlExistence(session, @event.Entity as Webpage))
                    {
                        var urlHistory = new UrlHistory
                        {
                            Webpage = webpage,
                            UrlSegment = Convert.ToString(@event.OldState[indexOf]),
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow,
                            Site = session.Get<Site>(CurrentRequestData.CurrentSite.Id)
                        };

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

        private bool CheckUrlExistence(ISession session, Document document)
        {
            return session.QueryOver<UrlHistory>().Where(doc => doc.UrlSegment == document.UrlSegment).RowCount() > 0;
        }
    }
}
