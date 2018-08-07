using System;
using AutoMapper;
using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class IdToEntityResolver<TModel, TEntity, TMappedEntity> : IValueResolver<TModel, TEntity, TMappedEntity> where TMappedEntity : SystemEntity
    {
        private readonly ISession _session;

        protected IdToEntityResolver(ISession session)
        {
            _session = session;
        }

        public Func<TModel, int?> GetId { get; set; } = model => null;

        public TMappedEntity Resolve(TModel source, TEntity destination, TMappedEntity destMember, ResolutionContext context)
        {
            var id = GetId(source);
            return id.HasValue
                ? _session.Get<TMappedEntity>(id.Value)
                : null;
        }
    }
}