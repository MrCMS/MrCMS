using System;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using NHibernate.Event;
using NHibernate.Persister.Entity;

namespace MrCMS.DbConfiguration.Configuration
{
    [Serializable]
    public class SetCoreProperties : IPreInsertEventListener
    {
        public async Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
            return OnPreInsert(@event);
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            var systemEntity = @event.Entity as SystemEntity;
            if (systemEntity != null)
            {
                var now = DateTime.UtcNow;

                if (systemEntity.CreatedOn == DateTime.MinValue)
                    SetCreatedOn(@event.Persister, @event.State, systemEntity, now);

                if (systemEntity.UpdatedOn == DateTime.MinValue)
                    SetUpdatedOn(@event.Persister, @event.State, systemEntity, now);

                if (systemEntity is SiteEntity && (systemEntity as SiteEntity).Site == null)
                {
                    // TODO: get site
                    Site site = null; //CurrentRequestData.CurrentSite;
                    SetSite(@event.Persister, @event.State, systemEntity as SiteEntity, site);
                }
            }

            return false;
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
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}