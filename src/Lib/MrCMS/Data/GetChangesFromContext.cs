using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public class GetChangesFromContext : IGetChangesFromContext
    {
        public ContextChangeData GetChanges<T>(DbContext context) where T : class
        {
            var entityEntries = context.ChangeTracker.Entries<T>().ToList();
            return new ContextChangeData
            {
                Added = GetSimpleEntries(entityEntries.FindAll(x => x.State == EntityState.Added)),
                Updated = GetChangeInfo(entityEntries.FindAll(x => x.State == EntityState.Modified)),
                Deleted = GetSimpleEntries(entityEntries.FindAll(x => x.State == EntityState.Deleted))
            };
        }

        private ICollection<ChangeInfo> GetChangeInfo<T>(List<EntityEntry<T>> entityEntries) where T : class
        {
            var changeInfos = new List<ChangeInfo>();
            foreach (var entry in entityEntries)
            {

                var changeInfo = new ChangeInfo
                {
                    Type = entry.Metadata.ClrType,
                    Entity = () => entry.Entity,
                    EntityType = GetTypeName(entry),
                    OriginalValues = GetValueDictionary(entry.OriginalValues),
                    Properties = GetValueDictionary(entry.CurrentValues)
                };
                // nothing changed then we don't raise data
                if (!changeInfo.PropertiesUpdated.Any())
                    continue;

                changeInfos.Add(changeInfo);
            }
            return changeInfos;
        }

        private IImmutableDictionary<string, object> GetValueDictionary(PropertyValues propertyValues)
        {
            return propertyValues.Properties.ToImmutableDictionary(x => x.Name, x => propertyValues[x]);
        }

        private ICollection<EntityData> GetSimpleEntries<T>(List<EntityEntry<T>> entityEntries) where T : class
        {
            return entityEntries.Select(entry => new EntityData
            {
                Type = entry.Metadata.ClrType,
                EntityType = GetTypeName(entry),
                Entity = () => entry.Entity,
                Properties = GetValueDictionary(entry.CurrentValues)
            }).ToList();
        }

        private static string GetTypeName<T>(EntityEntry<T> entry) where T : class
        {
            var type = entry.Metadata;
            while (type.BaseType != null)
                type = type.BaseType;
            return type.ClrType?.Name;
        }
    }
}