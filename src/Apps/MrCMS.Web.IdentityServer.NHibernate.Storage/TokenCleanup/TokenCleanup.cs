using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;
using MrCMS.Web.IdentityServer.NHibernate.Storage.TokenCleanup;
using NHibernate;

namespace IdentityServer4.NHibernate.TokenCleanup
{
    /// <summary>
    /// Periodically cleans up expired persisted grants.
    /// </summary>
    public class TokenCleanup
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanup> _logger;
        private readonly OperationalStoreOptions _options;

        private CancellationTokenSource _source;

        /// <summary>
        /// Initializes a TokenCleanup instance.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">Configuration options.</param>
        public TokenCleanup(IServiceProvider serviceProvider, ILogger<TokenCleanup> logger, OperationalStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (_options.TokenCleanupInterval < 1)
            {
                throw new ArgumentException("Token cleanup interval must be at least 1 second");
            }

            if (_options.TokenCleanupBatchSize < 1)
            {
                throw new ArgumentException("Token cleanup batch size interval must be at least 1");
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private TimeSpan CleanupInterval => TimeSpan.FromSeconds(_options.TokenCleanupInterval);

        /// <summary>
        /// Starts the token cleanup task.
        /// </summary>
        public void Start()
        {
            Start(CancellationToken.None);
        }

        /// <summary>
        /// Starts the token cleanup task.
        /// </summary>
        public void Start(CancellationToken cancellationToken)
        {
            if (_source != null) throw new InvalidOperationException("TokenCleanup task already started. Call Stop() first.");

            _logger.LogDebug("TokenCleanup - Starting token cleanup.");

            _source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Factory.StartNew(() => StartInternal(_source.Token));
        }

        /// <summary>
        /// Stops the token cleanup task.
        /// </summary>
        public void Stop()
        {
            if (_source == null) throw new InvalidOperationException("TokenCleanup task not started. Call Start() first.");

            _logger.LogDebug("TokenCleanup - Stopping token cleanup.");

            _source.Cancel();
            _source = null;
        }

        private async Task StartInternal(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                try
                {
                    await Task.Delay(CleanupInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug("TaskCanceledException. Exiting.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Task.Delay exception: {0}. Exiting.", ex.Message);
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                await RemoveExpiredGrantsAsync();
            }
        }

        /// <summary>
        /// Performs the actual cleanup.
        /// </summary>
        private async Task RemoveExpiredGrantsAsync()
        {
            string deleteExpiredTokensHql = "delete PersistedGrant pg where pg.ID in (:expiredTokensIDs)";

            try
            {
                _logger.LogTrace("TokenCleanup - Querying for expired grants to clear");

                var found = int.MaxValue;

                using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var tokenCleanupNotification = serviceScope.ServiceProvider.GetService<IOperationalStoreNotification>();

                    using (var session = serviceScope.ServiceProvider.GetService<ISession>())
                    {
                        while (found >= _options.TokenCleanupBatchSize)
                        {
                            using (var tx = session.BeginTransaction())
                            {
                                var expiredTokensQuery = session.QueryOver<PersistedGrant>()
                                    .Where(g => g.CreationTime < DateTimeOffset.UtcNow)
                                    .OrderBy(g => g.Id).Asc
                                    .Select(g => g.Id)
                                    .Take(_options.TokenCleanupBatchSize);

                                var expiredTokensIDs = (await expiredTokensQuery.ListAsync()).ToArray();
                                found = expiredTokensIDs.Length;

                                if (found > 0)
                                {
                                    _logger.LogInformation($"Removing {found} expired grants");

                                    await session.CreateQuery(deleteExpiredTokensHql)
                                        .SetParameterList("expiredTokensIDs", expiredTokensIDs)
                                        .ExecuteUpdateAsync();

                                    await tx.CommitAsync();

                                    if (tokenCleanupNotification != null)
                                    {
                                        await tokenCleanupNotification.PersistedGrantsRemovedAsync(expiredTokensIDs);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TokenCleanup - Exception clearing tokens: {ex.Message}");
            }
        }
    }
}
