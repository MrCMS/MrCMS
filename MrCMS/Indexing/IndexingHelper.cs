using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;

namespace MrCMS.Indexing
{
    public static class IndexingHelper
    {
        public static bool AnyIndexes<T>()
        {
            return GetDefinitionTypes<T>().Any() || GetRelatedDefinitionTypes<T>().Any();
        }
        public static bool AnyIndexes(object obj)
        {
            if (obj == null)
                return false;
            return DefinitionTypes(obj.GetType()).Any() || RelatedDefinitionTypes(obj.GetType()).Any();
        }
        public static List<Type> GetDefinitionTypes<T>()
        {
            return DefinitionTypes(typeof(T));
        }

        private static List<Type> DefinitionTypes(Type typeToCheck)
        {
            var definitionTypes = IndexDefinitionTypes.Where(type =>
                                                                 {
                                                                     var indexDefinitionInterface =
                                                                         type.GetInterfaces()
                                                                             .FirstOrDefault(
                                                                                 interfaceType =>
                                                                                 interfaceType.IsGenericType &&
                                                                                 interfaceType.GetGenericTypeDefinition() ==
                                                                                 typeof (IIndexDefinition<>));
                                                                     var genericArgument =
                                                                         indexDefinitionInterface.GetGenericArguments()[
                                                                             0];

                                                                     return
                                                                         genericArgument.IsAssignableFrom(typeToCheck);
                                                                 }).ToList();
            return definitionTypes;
        }

        public static List<Type> GetRelatedDefinitionTypes<T>()
        {
            return RelatedDefinitionTypes(typeof (T));
        }

        private static List<Type> RelatedDefinitionTypes(Type typeToCheck)
        {
            var definitionTypes = IndexDefinitionTypes.Where(type =>
                                                                 {
                                                                     var interfaces =
                                                                         type.GetInterfaces()
                                                                             .Where(
                                                                                 interfaceType =>
                                                                                 interfaceType.IsGenericType &&
                                                                                 interfaceType.GetGenericTypeDefinition() ==
                                                                                 typeof (IRelatedItemIndexDefinition<,>));

                                                                     return
                                                                         interfaces.Any(
                                                                             relatedDefinitionType =>
                                                                             relatedDefinitionType.GetGenericArguments()
                                                                                 [0].IsAssignableFrom(typeToCheck));
                                                                 }).ToList();
            return definitionTypes;
        }

        private static List<Type> IndexDefinitionTypes
        {
            get { return TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IIndexDefinition<>)); }
        }
    }
}