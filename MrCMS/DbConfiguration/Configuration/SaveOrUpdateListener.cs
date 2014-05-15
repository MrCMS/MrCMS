using System;
using System.Web;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SaveOrUpdateListener : DefaultSaveOrUpdateEventListener, IPreUpdateEventListener, IPreInsertEventListener
    {
        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var systemEntity = @event.Entity as SystemEntity;
            if (systemEntity != null)
            {
                var now = CurrentRequestData.Now;
                if (!DbHasSetCreatedOn(@event.Persister, @event.State))
                    SetCreatedOn(@event.Persister, @event.State, systemEntity, now);
                SetUpdatedOn(@event.Persister, @event.State, systemEntity, now);
            }
            return false;
        }

        private bool DbHasSetCreatedOn(IEntityPersister persister, object[] state)
        {
            int index = Array.IndexOf(persister.PropertyNames, "CreatedOn");

            var currentCreatedOn = (DateTime)state[index];
            DateTime defaultDateTime = default(DateTime);
            return !DateTime.Equals(currentCreatedOn, defaultDateTime);
        }

        private void SetUpdatedOn(IEntityPersister persister, object[] state, SystemEntity siteEntity, DateTime date)
        {
            Set(persister, state, "UpdatedOn", date);

            siteEntity.UpdatedOn = date;
        }
        private void SetCreatedOn(IEntityPersister persister, object[] state, SystemEntity siteEntity, DateTime date)
        {
            Set(persister, state, "CreatedOn", date);

            siteEntity.CreatedOn = date;
        }

        private void SetSite(IEntityPersister persister, object[] state, SiteEntity siteEntity, Site site)
        {
            Set(persister, state, "Site", site);

            siteEntity.Site = site;
        }

        private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            int index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            var systemEntity = @event.Entity as SystemEntity;
            if (systemEntity != null)
            {
                var now = CurrentRequestData.Now;
                SetCreatedOn(@event.Persister, @event.State, systemEntity, now);
                SetUpdatedOn(@event.Persister, @event.State, systemEntity, now);
                if (systemEntity is SiteEntity && (systemEntity as SiteEntity).Site == null)
                    SetSite(@event.Persister, @event.State, systemEntity as SiteEntity, CurrentRequestData.CurrentSite);
            }
            return false;
        }
    }
}
