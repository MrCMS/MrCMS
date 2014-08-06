using Iesi.Collections;
using MrCMS.Entities;
using MrCMS.Tasks;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SoftDeleteListener : DefaultDeleteEventListener
    {
        protected override void DeleteEntity(IEventSource session, object entity, EntityEntry entityEntry,
            bool isCascadeDeleteEnabled, IEntityPersister persister, ISet transientEntities)
        {
            if (entity is SystemEntity)
            {
                var e = (SystemEntity) entity;
                e.IsDeleted = true;

                CascadeBeforeDelete(session, persister, entity, entityEntry, transientEntities);
                CascadeAfterDelete(session, persister, entity, transientEntities);
            }
            else
            {
                base.DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled,
                    persister, transientEntities);
            }
        }
    }
}