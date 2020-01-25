using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public class EntityData
    {
        public Type Type { get; set; }

        public IEnumerable<Type> TypeHierarchy
        {
            get
            {
                var type = Type;
                while (type != null)
                {
                    yield return type;
                    type = type.BaseType;
                }
            }
        }
        public string EntityType { get; set; }
        /// <summary>
        /// This is a function so that we can point to the id for after it's added
        /// </summary>
        public Func<object> Entity { get; set; }

        public int EntityId => Entity?.Invoke() is IHaveId id ? id.Id : -1;
        public IImmutableDictionary<string, object> Properties { get; set; }
    }
}