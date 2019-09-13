using System;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Stores
{
    /// <summary>
    /// Implementation of the NHibernate-based client store.
    /// </summary>
    public class ClientStore : IClientStore
    {
        private readonly ISession _session;
        private readonly ILogger<ClientStore> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session used to retrieve the data.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The Mapper.</param>
        /// <exception cref="ArgumentNullException">session</exception>
        public ClientStore(ISession session, ILogger<ClientStore> logger, IMapper mapper)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Finds a client by its client id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public async Task<IdentityServer4.Models.Client> FindClientByIdAsync(string clientId)
        {
            Client client = null;
            using (var tx = _session.BeginTransaction())
            {
                var clientQuery = _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(SelectMode.Fetch, c => c.AllowedGrantTypes)
                    .Fetch(SelectMode.Fetch, c => c.ClientSecrets)
                    .Fetch(SelectMode.Fetch, c => c.AllowedScopes)
                    .Fetch(SelectMode.Fetch, c => c.Claims)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .FutureValue<Client>();

                _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(SelectMode.Fetch, c => c.RedirectUris)
                    .Fetch(SelectMode.Fetch, c => c.PostLogoutRedirectUris)
                    .Fetch(SelectMode.Fetch, c => c.AllowedCorsOrigins)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .FutureValue<Client>();

                _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(SelectMode.Fetch, c => c.IdentityProviderRestrictions)
                    .Fetch(SelectMode.Fetch, c => c.Properties)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .FutureValue<Client>();

                client = await clientQuery.GetValueAsync();

                await tx.CommitAsync();
            }

            
            IdentityServer4.Models.Client clientModel = new IdentityServer4.Models.Client();
            _mapper.Map(client, clientModel);

            _logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, clientModel != null);

            return clientModel;
        }
    }
}
