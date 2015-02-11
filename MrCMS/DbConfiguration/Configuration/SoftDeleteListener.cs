using System;
using System.Collections.Generic;
using MrCMS.Entities;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;

namespace MrCMS.DbConfiguration.Configuration
{
    [Serializable]
    public class SoftDeleteListener : DefaultDeleteEventListener
    {
        protected override void DeleteEntity(IEventSource session, object entity, EntityEntry entityEntry,
            bool isCascadeDeleteEnabled, IEntityPersister persister, ISet<object> objects)
        {
            if (entity is SystemEntity)
            {
                var e = (SystemEntity) entity;
                e.IsDeleted = true;

                CascadeBeforeDelete(session, persister, entity, entityEntry, objects);
                CascadeAfterDelete(session, persister, entity, objects);
            }
            else
            {
                base.DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled,
                    persister, objects);
            }
        }
    }
}