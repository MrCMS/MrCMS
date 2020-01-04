using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Entities;
using MrCMS.Entities.Audit;
using MrCMS.Services;
using MrCMS.Settings;
using Newtonsoft.Json;

namespace MrCMS.Data
{
    public class AuditChanges : IAuditChanges
    {
        private readonly DbContext _dbContext;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IGetCurrentUser _getCurrentUser;

        public AuditChanges(IMrCmsContextResolver contextResolver, IConfigurationProvider configurationProvider, IGetCurrentUser getCurrentUser)
        {
            _dbContext = contextResolver.Resolve();
            _configurationProvider = configurationProvider;
            _getCurrentUser = getCurrentUser;
        }

        public async Task Audit(ContextChangeData data)
        {
            var auditSettings = _configurationProvider.GetSiteSettings<AuditSettings>();
            var auditLevel = auditSettings.AuditLevel;

            if (auditLevel == AuditLevel.None)
                return;

            var userId = _getCurrentUser.Get()?.Id;
            foreach (var entry in data.Added.Where(entityData => ShouldAudit(entityData, auditLevel, AuditOperations.Add)))
                await _dbContext.AddAsync(new EntityAdded
                {
                    UserId = userId,
                    EntityId = entry.EntityId,
                    EntityType = entry.EntityType
                });

            foreach (var entry in data.Deleted.Where(entityData => ShouldAudit(entityData, auditLevel, AuditOperations.Delete)))
                await _dbContext.AddAsync(new EntityDeleted
                {
                    UserId = userId,
                    EntityId = entry.EntityId,
                    EntityType = entry.EntityType
                });

            foreach (var entry in data.Updated.Where(entityData => ShouldAudit(entityData, auditLevel, AuditOperations.Update)))
            {
                var entityUpdated = new EntityUpdated
                {
                    UserId = userId,
                    EntityId = entry.EntityId,
                    EntityType = entry.EntityType
                };
                await _dbContext.AddAsync(entityUpdated);

                if (ShouldAudit(entry, auditLevel, AuditOperations.Properties))
                    await _dbContext.AddRangeAsync(entry.PropertiesUpdated.Select(propertyData => new PropertyUpdated
                    {
                        EntityUpdated = entityUpdated,
                        Name = propertyData.Name,
                        OriginalValue = propertyData.OriginalValue == null
                            ? null
                            : JsonConvert.SerializeObject(propertyData.OriginalValue),
                        CurrentValue = propertyData.CurrentValue == null
                            ? null
                            : JsonConvert.SerializeObject(propertyData.CurrentValue)
                    }));
            }

            await _dbContext.SaveChangesAsync();
        }

        private bool ShouldAudit(EntityData entityData, AuditLevel auditLevel, AuditOperations operations)
        {
            if (auditLevel == AuditLevel.None)
                return false;

            var attribute = entityData.Type.GetTypeInfo().GetCustomAttribute<AuditableAttribute>(true);
            if (attribute?.Operations.HasFlag(AuditOperations.Never) == true)
                return false;

            if (auditLevel == AuditLevel.DebugFull)
                return true;

            if (auditLevel == AuditLevel.DebugSimple || attribute == null)
                return operations != AuditOperations.Properties;

            return attribute.Operations.HasFlag(operations);
        }
    }
}