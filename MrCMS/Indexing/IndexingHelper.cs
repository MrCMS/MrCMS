using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
using MrCMS.Website;

namespace MrCMS.Indexing
{
    public static class IndexingHelper
    {
        public static bool AnyIndexes(object obj, LuceneOperation operation)
        {
            if (obj == null)
                return false;
            return
                IndexDefinitionTypes.Any(
                    definition => definition.GetUpdateTypes(operation).Any(type => type.IsAssignableFrom(obj.GetType())));
        }

        public static List<IndexDefinition> IndexDefinitionTypes
        {
            get
            {
                return
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IndexDefinition<>))
                        .Select(type => MrCMSApplication.Get(type) as IndexDefinition)
                        .ToList();
            }
        }

        public static IndexDefinition Get<T>() where T :IndexDefinition
        {
            return IndexDefinitionTypes.OfType<T>().FirstOrDefault();
        }
    }
}