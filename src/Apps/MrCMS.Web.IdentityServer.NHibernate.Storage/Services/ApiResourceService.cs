using System;
using MrCMS.Helpers;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using NHibernate;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    public class ApiResourceService : IApiResourceService
    {
        private readonly ISession _session;
        public ApiResourceService(ISession session)
        {
            _session = session;
        }
        public void Add(ApiResource resource)
        {
            _session.Transact(session =>
            {
                session.Save(resource);
                foreach (var item in resource.Secrets)
                {
                    item.ApiResource = resource;
                    session.SaveOrUpdate(item);
                }
                foreach (var item in resource.Scopes)
                {
                    item.ApiResource = resource;
                    session.SaveOrUpdate(item);
                    foreach(var item2 in item.UserClaims)
                    {
                        item2.ApiScope = item;
                        session.SaveOrUpdate(item2);
                    }
                }

                foreach (var item in resource.UserClaims)
                {
                    item.ApiResource = resource;
                    session.SaveOrUpdate(item);
                }

                foreach (var item in resource.Properties)
                {
                    item.ApiResource = resource;
                    session.SaveOrUpdate(item);
                }
            });

           
        }

        public void Update(ApiResource resource)
        {
            throw new NotImplementedException();
        }
    }
}