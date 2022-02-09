using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.TextSearch.EntityConverters;

namespace MrCMS.TextSearch.Services
{
    internal static class TextSearchConverterMap
    {
        internal static Dictionary<Type, Type> Types { get; set; }

        static TextSearchConverterMap()
        {
            Types = new Dictionary<Type, Type>();
            var allTypes = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(BaseTextSearchEntityConverter<>));

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<SystemEntity>()
                .Where(type => !type.ContainsGenericParameters))
            {
                var thisType = type;
                // this will try and find matches for each type starting with the most specific and working up
                while (typeof(SystemEntity).IsAssignableFrom(thisType))
                {
                    var converterType = allTypes.FirstOrDefault(x =>
                        typeof(BaseTextSearchEntityConverter<>).MakeGenericType(thisType).IsAssignableFrom(x));
                    if (converterType != null)
                    {
                        Types.Add(type, converterType);
                        break;
                    }

                    thisType = thisType.BaseType;
                }
            }
        }
    }
}