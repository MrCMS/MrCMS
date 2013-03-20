using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Website;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UrlHistoryListener : IPostUpdateEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            try
            {
                var session = @event.Session.SessionFactory.OpenSession();
                if (@event.Entity is Document)
                {
                    var webpage = GetWebpage(session, @event.Entity as Document);

                    if (!CheckUrlExistence(session, @event.Entity as Document))
                    {
                        var urlHistory = new UrlHistory
                        {
                            Webpage=webpage,
                            UrlSegment=webpage.UrlSegment,
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

        private Webpage GetWebpage(ISession session, Document document)
        {
            return
                session.QueryOver<Webpage>().Fetch(doc => doc.Urls).Eager.Where(
                    doc => doc.Id == document.Id).List().FirstOrDefault();
        }

        private bool CheckUrlExistence(ISession session, Document document)
        {
            if (session.QueryOver<UrlHistory>().Where(doc => doc.UrlSegment == document.UrlSegment).List().Count() > 0)
                return true;
            else
                return false;
        }
    }
}
