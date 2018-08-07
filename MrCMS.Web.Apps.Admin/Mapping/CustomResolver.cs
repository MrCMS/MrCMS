using AutoMapper;
using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class CustomResolver<TSource, TDestination, TEntity> : IMemberValueResolver<TSource, TDestination, int?,TEntity> where TEntity : SystemEntity
    {
        private readonly ISession _session;

        public CustomResolver(ISession session)
        {
            _session = session;
        }
        public TEntity Resolve(TSource source, TDestination destination, int? sourceMember, TEntity destMember,
            ResolutionContext context)
        {
            return !sourceMember.HasValue ? null : _session.Get<TEntity>(sourceMember.Value);
        }
    }
}