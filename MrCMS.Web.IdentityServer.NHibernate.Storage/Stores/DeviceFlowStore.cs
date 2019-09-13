using System;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using NHibernate;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Stores
{
    /// <summary>
    /// Implementation of the NHibernate-based device flow store.
    /// </summary>
    public class DeviceFlowStore : IDeviceFlowStore
    {
        private readonly ISession _session;
        private readonly ILogger _logger;
        private readonly IPersistentGrantSerializer _serializer;
        private readonly IMapper _mapper;


        /// <summary>
        /// Creates a new instance of the <see cref="DeviceFlowStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session.</param>
        /// <param name="serializer">The serializer</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The Mapper.</param>
        public DeviceFlowStore(ISession session, IPersistentGrantSerializer serializer, ILogger<DeviceFlowStore> logger, IMapper mapper)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Finds device authorization by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            DeviceCode model = null;
            using (var tx = _session.BeginTransaction())
            {
                var query = _session.QueryOver<DeviceFlowCodes>()
                    .Where(c => c.DeviceCode == deviceCode);

                var result = await query.SingleOrDefaultAsync();
                model = ToModel(result?.Data);
            }

            _logger.LogDebug("{deviceCode} found in database: {deviceCodeFound}", deviceCode, model != null);

            return model;
        }

        /// <summary>
        /// Finds device authorization by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        public async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            DeviceCode model = null;
            using (var tx = _session.BeginTransaction())
            {
                var result = await _session.GetAsync<DeviceFlowCodes>(userCode);
                model = ToModel(result?.Data);
            }

            _logger.LogDebug("{userCode} found in database: {userCodeFound}", userCode, model != null);

            return model;
        }

        /// <summary>
        /// Removes the device authorization, searching by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            using (var tx = _session.BeginTransaction())
            {
                var deviceFlowCodes = await _session.QueryOver<DeviceFlowCodes>()
                    .Where(c => c.DeviceCode == deviceCode)
                    .SingleOrDefaultAsync();

                if (deviceFlowCodes != null)
                {
                    _logger.LogDebug("removing {deviceCode} device code from database", deviceCode);

                    try
                    {
                        await _session.DeleteAsync(deviceFlowCodes);
                        await tx.CommitAsync();
                    }
                    catch (HibernateException ex)
                    {
                        _logger.LogInformation("exception removing {deviceCode} device code from database: {error}", deviceCode, ex.Message);
                    }
                }
                else
                {
                    _logger.LogDebug("no {deviceCode} device code found in database", deviceCode);
                }
            }
        }

        /// <summary>
        /// Stores the device authorization request.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        public async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            using (var tx = _session.BeginTransaction())
            {
                await _session.SaveAsync(ToEntity(data, deviceCode, userCode));
                await tx.CommitAsync();
            }
        }

        /// <summary>
        /// Updates device authorization, searching by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            using (var tx = _session.BeginTransaction())
            {
                var codeToUpdate = _session.Get<DeviceFlowCodes>(userCode);
                if (codeToUpdate == null)
                {
                    _logger.LogError("{userCode} not found in database", userCode);
                    throw new InvalidOperationException("Could not update device code");
                }

                var entity = ToEntity(data, codeToUpdate.DeviceCode, userCode);
                _logger.LogDebug("{userCode} found in database", userCode);

                codeToUpdate.SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value;
                codeToUpdate.Data = entity.Data;

                try
                {
                    await _session.SaveAsync(codeToUpdate);
                    await tx.CommitAsync();
                }
                catch (HibernateException ex)
                {
                    _logger.LogWarning("exception updating {userCode} user code in database: {error}", userCode, ex.Message);
                }
            }
        }

        /// <summary>
        /// Maps a <see cref="DeviceCode"/> to a <see cref="DeviceFlowCodes"/> instance.
        /// </summary>
        private DeviceFlowCodes ToEntity(DeviceCode model, string deviceCode, string userCode)
        {
            if (model == null || deviceCode == null || userCode == null) return null;

            return new DeviceFlowCodes
            {
                DeviceCode = deviceCode,
                UserCode = userCode,
                ClientId = model.ClientId,
                SubjectId = model.Subject?.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = model.CreationTime,
                Expiration = model.CreationTime.AddSeconds(model.Lifetime),
                Data = _serializer.Serialize(model)
            };
        }

        /// <summary>
        /// Maps a device code to a <see cref="DeviceCode"/> instance.
        /// </summary>
        private DeviceCode ToModel(string entity)
        {
            if (entity == null) return null;

            return _serializer.Deserialize<DeviceCode>(entity);
        }
    }
}
