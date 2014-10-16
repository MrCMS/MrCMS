using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Search.Models
{
    public class UniversalSearchItem
    {
        public string SystemType { get; set; }
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ActionUrl { get; set; }
        public Guid? SearchGuid { get; set; }
        public IEnumerable<string> SearchTerms { get; set; }

        public IEnumerable<string> EntityTypes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SystemType))
                    yield break;
                Type entityType = TypeHelper.GetTypeByName(SystemType);
                while (entityType != null && typeof (SystemEntity).IsAssignableFrom(entityType))
                {
                    yield return entityType.FullName;
                    entityType = entityType.BaseType;
                }
            }
        }
    }
}