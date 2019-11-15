using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.People;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    public class DeveloperIS4Service : IDeveloperIS4Service
    {
        private readonly ISession _session;
        private readonly ILogger _logger;
        private readonly IClientService _clientService;
        private readonly IIdentityResourceService _identityResourceService;
        private readonly IApiResourceService _apiResourceService;
        private readonly IMapper _mapper;
        public DeveloperIS4Service(ISession session, ILogger<DeveloperIS4Service> logger, IClientService clientService, IMapper mapper,
            IIdentityResourceService identityResourceService, IApiResourceService apiResourceService)
        {
            _session = session;
            _logger = logger;
            _clientService = clientService;
            _mapper = mapper;
            _identityResourceService = identityResourceService;
            _apiResourceService = apiResourceService;
        }
        public void InitializeDatabaseData()
        {

           
            var clients = _session.QueryOver<Client>().List();
            if (!clients.Any())
            {
                _logger.LogInformation("Initializing IS4 Developer Data");
                foreach (var client in Config.GetClients())
                {
                    var newClient = _mapper.Map<Client>(client);
                    var newClientJson = JsonConvert.SerializeObject(newClient);
                    _logger.LogInformation("Initializing IS4 Developer Data");
                    //  _logger.LogInformation(newClientJson);
                    _clientService.Add(_mapper.Map<Client>(client));
                    //using (var tx = _session.BeginTransaction())
                    //{
                    //    _session.Save(newClient);
                    //    _session.Flush();
                    //    tx.Commit();
                    //}
                }

                var identityresources = _session.QueryOver<IdentityResource>().List();
                if (!identityresources.Any())
                {
                    _logger.LogInformation("Add Default Identity resources");
                    foreach (var identity in Config.GetIdentityResources())
                    {
                        _identityResourceService.Add(_mapper.Map<IdentityResource>(identity));
                    }
                }

                var apiresources = _session.QueryOver<ApiResource>().List();
                if (!apiresources.Any())
                {
                    _logger.LogInformation("Add Default Api resources");
                    foreach (var api in Config.GetApis())
                    {
                        _apiResourceService.Add(_mapper.Map<ApiResource>(api));
                    }
                }

            }
            else
            {
                _logger.LogInformation("IS4 Developer Data already initialized");
            }
        }
    }
}
