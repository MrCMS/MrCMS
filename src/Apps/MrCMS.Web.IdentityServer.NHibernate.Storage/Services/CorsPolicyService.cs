using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    /// <summary>
    /// Implementation of NHibernate-based CorsPolicyService.
    /// Checks the client configuration on the database for allowed CORS origins.
    /// </summary>
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IHttpContextAccessor _context;
        private readonly ILogger<CorsPolicyService> _logger;

        /// <summary>
        /// Creates a new instance of the NHibernate-based Cors Policy Service.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <param name="logger">The logger.</param>
        public CorsPolicyService(IHttpContextAccessor context, ILogger<CorsPolicyService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        /// <summary>
        /// Determines whether origin is allowed or not.
        /// </summary>
        /// <param name="origin">The origin.</param>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            bool isAllowed = false;

            using (var session = _context.HttpContext.RequestServices.GetRequiredService<IStatelessSession>())
            {
                ClientCorsOrigin corsOriginAlias = null;
                var corsOriginsQuery = session.QueryOver<Client>()
                    .JoinQueryOver(c => c.AllowedCorsOrigins, () => corsOriginAlias)
                    .Where(() => corsOriginAlias.Origin == origin)
                    .Select(Projections.Distinct(
                        Projections.ProjectionList()
                            .Add(Projections.Property<ClientCorsOrigin>(o => corsOriginAlias.Origin))
                    ));

                var origins = await corsOriginsQuery.ListAsync<string>();

                isAllowed = origins.Any();
            }

            _logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

            return isAllowed;
        }
    }
}