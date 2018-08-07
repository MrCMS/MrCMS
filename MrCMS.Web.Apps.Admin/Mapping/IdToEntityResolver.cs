using AutoMapper;
using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public abstract class IdToEntityResolver<TModel, TEntity, TMappedEntity> : IValueResolver<TModel, TEntity, TMappedEntity> where TEntity : SystemEntity where TMappedEntity : SystemEntity
    {
        private readonly ISession _session;

        protected IdToEntityResolver(ISession session)
        {
            _session = session;
        }

        public TMappedEntity Resolve(TModel source, TEntity destination, TMappedEntity destMember, ResolutionContext context)
        {
            var id = GetId(source);
            return id.HasValue
                ? _session.Get<TMappedEntity>(id.Value)
                : null;
        }

        protected abstract int? GetId(TModel source);
    }
}