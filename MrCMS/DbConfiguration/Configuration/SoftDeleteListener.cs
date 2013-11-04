using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Tasks;
using NHibernate.Event.Default;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SoftDeleteListener : DefaultDeleteEventListener
    {
        protected override void DeleteEntity(NHibernate.Event.IEventSource session, object entity, NHibernate.Engine.EntityEntry entityEntry, bool isCascadeDeleteEnabled, NHibernate.Persister.Entity.IEntityPersister persister, Iesi.Collections.ISet transientEntities)
        {
            if (entity is SystemEntity)
            {
                var e = (SystemEntity)entity;
                e.IsDeleted = true;

                CascadeBeforeDelete(session, persister, entity, entityEntry, transientEntities);
                CascadeAfterDelete(session, persister, entity, transientEntities);

                var siteEntity = e as SiteEntity;
                if (siteEntity != null)
                    TaskExecutor.ExecuteLater(UpdateIndicesListener.Create(typeof(DeleteIndicesTask<>), siteEntity));
            }
            else
            {
                base.DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled,
                                  persister, transientEntities);
            }
        }
    }
}