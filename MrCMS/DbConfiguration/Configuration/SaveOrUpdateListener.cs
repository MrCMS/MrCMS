using System;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Events;
using MrCMS.Website;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SaveOrUpdateListener : DefaultSaveOrUpdateEventListener, IPreUpdateEventListener,
        IPreInsertEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            //var systemEntity = @event.Entity as SystemEntity;
            //if (systemEntity != null)
            //{
            //    var now = CurrentRequestData.Now;
            //    SetCreatedOn(@event.Persister, @event.State, systemEntity, now);
            //    SetUpdatedOn(@event.Persister, @event.State, systemEntity, now);
            //    if (systemEntity is SiteEntity && (systemEntity as SiteEntity).Site == null)
            //        SetSite(@event.Persister, @event.State, systemEntity as SiteEntity, CurrentRequestData.CurrentSite);
            //}
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            //var systemEntity = @event.Entity as SystemEntity;
            //if (systemEntity != null)
            //{
            //    var now = CurrentRequestData.Now;
            //    if (!DbHasSetCreatedOn(@event.Persister, @event.State))
            //        SetCreatedOn(@event.Persister, @event.State, systemEntity, now);
            //    SetUpdatedOn(@event.Persister, @event.State, systemEntity, now);
            //}
            return false;
        }

        private bool DbHasSetCreatedOn(IEntityPersister persister, object[] state)
        {
            int index = Array.IndexOf(persister.PropertyNames, "CreatedOn");

            var currentCreatedOn = (DateTime) state[index];
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
    }

    public class SetOnAddingProperties : IOnAdding
    {
        private readonly Site _site;

        public SetOnAddingProperties(Site site)
        {
            _site = site;
        }

        public void Execute(OnAddingArgs args)
        {
            DateTime now = CurrentRequestData.Now;
            SystemEntity systemEntity = args.Item;
            if (systemEntity.CreatedOn == DateTime.MinValue)
            {
                systemEntity.CreatedOn = now;
            }
            if (systemEntity.UpdatedOn == DateTime.MinValue)
            {
                systemEntity.UpdatedOn = now;
            }
            var siteEntity = systemEntity as SiteEntity;
            if (siteEntity == null) return;
            if (siteEntity.Site == null)
            {
                siteEntity.Site = _site;
            }
        }
    }
}