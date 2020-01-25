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
                Added = GetAdded(entityEntries),
                Updated = GetUpdated(entityEntries),
                Deleted = GetDeleted(entityEntries)
            };
        }


        private ICollection<ChangeInfo> GetUpdated<T>(List<EntityEntry<T>> entityEntries) where T : class
        {
            var changeInfos = new List<ChangeInfo>();
            foreach (var entry in entityEntries.Where(x => x.State == EntityState.Modified))
            {

                var changeInfo = GetChangeInfo(entry);
                // nothing changed then we don't raise data
                if (!changeInfo.PropertiesUpdated.Any())
                    continue;
                // if it's soft deleted also leave it
                var deletedChange = changeInfo.PropertiesUpdated.FirstOrDefault(x => x.Name == nameof(ICanSoftDelete.IsDeleted));
                if (deletedChange?.CurrentValue as bool? == true)
                    continue;

                changeInfos.Add(changeInfo);
            }
            return changeInfos;
        }

        private ChangeInfo GetChangeInfo<T>(EntityEntry<T> entry) where T : class
        {
            var changeInfo = new ChangeInfo
            {
                Type = entry.Metadata.ClrType,
                Entity = () => entry.Entity,
                EntityType = GetTypeName(entry),
                OriginalValues = GetValueDictionary(entry.OriginalValues),
                Properties = GetValueDictionary(entry.CurrentValues)
            };
            return changeInfo;
        }

        private IImmutableDictionary<string, object> GetValueDictionary(PropertyValues propertyValues)
        {
            return propertyValues.Properties.ToImmutableDictionary(x => x.Name, x => propertyValues[x]);
        }

        private ICollection<EntityData> GetDeleted<T>(List<EntityEntry<T>> entityEntries) where T : class
        {
            var entityDatas = new List<EntityData>();
            entityDatas.AddRange( entityEntries.Where(x => x.State == EntityState.Deleted).Select(entry => new EntityData
            {
                Type = entry.Metadata.ClrType,
                EntityType = GetTypeName(entry),
                Entity = () => entry.Entity,
                Properties = GetValueDictionary(entry.CurrentValues)
            }));
            foreach (var entry in entityEntries.Where(x => x.State == EntityState.Modified))
            {
                var changeInfo = GetChangeInfo(entry);

                // soft deleted only
                var deletedChange = changeInfo.PropertiesUpdated.FirstOrDefault(x => x.Name == nameof(ICanSoftDelete.IsDeleted));
                if (deletedChange?.CurrentValue as bool? != true)
                    continue;

                entityDatas.Add(changeInfo);
            }
            return entityDatas;
        }

        private ICollection<EntityData> GetAdded<T>(List<EntityEntry<T>> entityEntries) where T : class
        {
            return entityEntries.Where(x => x.State == EntityState.Added).Select(entry => new EntityData
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