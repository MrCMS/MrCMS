using System;
using MrCMS.Helpers;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using NHibernate;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    public class IdentityResourceService : IIdentityResourceService
    {
        private readonly ISession _session;
        public IdentityResourceService(ISession session)
        {
            _session = session;
        }
        public void Add(IdentityResource resource)
        {
            _session.Transact(session =>
            {
                session.Save(resource);

                foreach (var item in resource.UserClaims)
                {
                    item.IdentityResource = resource;
                    session.SaveOrUpdate(item);
                }

                foreach (var item in resource.Properties)
                {
                    item.IdentityResource = resource;
                    session.SaveOrUpdate(item);
                }

            });
        }

        public void Update(IdentityResource resource)
        {
            throw new NotImplementedException();
        }
    }
}