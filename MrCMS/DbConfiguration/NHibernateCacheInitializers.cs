using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration.Caches;
using MrCMS.Helpers;
using NHibernate.Cache;

namespace MrCMS.DbConfiguration
{
    public static class NHibernateCacheInitializers
    {
        static NHibernateCacheInitializers()
        {
            Initializers = new Dictionary<Type, Type>();

            foreach (Type type in TypeHelper.GetAllTypesAssignableFrom<ICacheProvider>().Where(type => !type.ContainsGenericParameters))
            {
                var thisType = type;
                var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(CacheProviderInitializer<>).MakeGenericType(thisType));
                if (types.Any())
                {
                    Initializers.Add(type, types.First());
                }
            }
        }

        public static Dictionary<Type, Type> Initializers { get; set; }
    }
}