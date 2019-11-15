using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.TokenCleanup
{
    /// <summary>
    /// Interface to model notifications from the TokenCleanup feature.
    /// </summary>
    public interface IOperationalStoreNotification
    {
        /// <summary>
        /// Notification for persisted grants being removed.
        /// </summary>
        /// <param name="persistedGrants"></param>
        Task PersistedGrantsRemovedAsync(IEnumerable<PersistedGrant> persistedGrants);
    }
}
