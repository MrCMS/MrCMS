using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MrCMS.Helpers;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    public interface IClientService
    {
        void Add(Client client);

        void Update(Client client);
    }

    public class ClientService : IClientService
    {
        private readonly ISession _session;
        public ClientService(ISession session)
        {
            _session = session;
        }
        public void Add(Client client)
        {
            _session.Transact(session =>
            {
                session.Save(client);


                foreach (var item in client.AllowedGrantTypes)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }
                foreach (var item in client.AllowedCorsOrigins)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }
                foreach (var item in client.AllowedScopes)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }
                foreach (var item in client.Claims)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }
                foreach (var item in client.AllowedCorsOrigins)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }

                foreach (var item in client.ClientSecrets)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }

                foreach (var item in client.RedirectUris)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }

                foreach (var item in client.PostLogoutRedirectUris)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }
                foreach (var item in client.IdentityProviderRestrictions)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }

                foreach (var item in client.AllowedCorsOrigins)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }

                foreach (var item in client.IdentityProviderRestrictions)
                {
                    item.Client = client;
                    session.SaveOrUpdate(item);
                }

            foreach (var item in client.Properties)
            {
                item.Client = client;
                session.SaveOrUpdate(item);
            }

      

    });

            //_session.Transact(session =>
            //{
            //    var propertyInfos = client.GetType().GetProperties();
            //    foreach (var item in propertyInfos)
            //    {
            //        if (item.PropertyType.IsGenericType == true && item.PropertyType.GetGenericTypeDefinition() == typeof(ISet<>))
            //        {

            //            var typeParams = item.GetValue(client);
            //            //var obj = JsonConvert.SerializeObject(typeParams);

            //            if (typeParams != null)
            //            {
            //                //  _logger.LogInformation(typeParams.GetType().FindMembers();
            //                if (typeParams is IEnumerable)
            //                {
            //                    foreach (var listitem in typeParams as IEnumerable)
            //                    {
            //                        // Console.WriteLine("Item: " + listitem.ToString());
            //                        var obj2 = JsonConvert.SerializeObject(listitem);
            //                        // _logger.LogInformation(obj2);
            //                        //_logger.LogInformation(listitem.GetType().Name);
            //                        var objtype = listitem.GetType().Name;
            //                        switch (objtype)
            //                        {
            //                            case "ClientSecret":
            //                                var clientSecret = JsonConvert.DeserializeObject<ClientSecret>(obj2);
            //                                clientSecret.Client = client;
            //                                session.SaveOrUpdate(clientSecret);
            //                                break;
            //                            case "ClientGrantType":
            //                                var clientGrantType = JsonConvert.DeserializeObject<ClientGrantType>(obj2);
            //                                clientGrantType.Client = client;
            //                                session.SaveOrUpdate(clientGrantType);
            //                                break;
            //                            case "ClientScope":
            //                                var clientScope = JsonConvert.DeserializeObject<ClientScope>(obj2);
            //                                clientScope.Client = client;
            //                                session.SaveOrUpdate(clientScope);
            //                                break;
            //                            default:
            //                                break;
            //                        };
            //                    }
            //                    // _logger.LogInformation(typeParams.GetType().Name);
            //                }
            //            }

            //        }

            //    }

            //});
        }

        public void Update(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
