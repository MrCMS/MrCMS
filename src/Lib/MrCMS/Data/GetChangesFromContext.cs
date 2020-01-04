using System.Collections.Generic;
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
                var properties = new List<PropertyData>();
                var originalValues = entry.OriginalValues;
                var currentValues = entry.CurrentValues;

                foreach (var property in currentValues.Properties)
                {
                    var originalValue = originalValues[property];
                    var currentValue = currentValues[property];

                    if (!Equals(originalValue, currentValue))
                    {
                        properties.Add(new PropertyData
                        {
                            Name = property.Name,
                            OriginalValue = originalValue,
                            CurrentValue = currentValue
                        });
                    }
                }

                if (!properties.Any())
                    continue;

                changeInfos.Add(new ChangeInfo
                {
                    Type = entry.Metadata.ClrType,
                    Entity = () => entry.Entity,
                    EntityType = GetTypeName(entry),
                    PropertiesUpdated = properties,
                    Properties = entry.CurrentValues
                });
            }
            return changeInfos;
        }

        private ICollection<EntityData> GetSimpleEntries<T>(List<EntityEntry<T>> entityEntries) where T : class
        {
            return entityEntries.Select(entry => new EntityData
            {
                Type = entry.Metadata.ClrType,
                EntityType = GetTypeName(entry),
                Entity = () => entry.Entity,
                Properties = entry.CurrentValues
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