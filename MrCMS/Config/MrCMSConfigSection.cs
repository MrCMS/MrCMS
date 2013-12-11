using System;
using System.ComponentModel;
using System.Configuration;
using NHibernate.Caches.SysCache2;

namespace MrCMS.Config
{
    public class MrCMSConfigSection : ConfigurationSection
    {
        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("cache-provider", DefaultValue = typeof(SysCacheProvider))]
        public Type CacheProvider
        {
            get { return (Type)this["cache-provider"]; }
            set { this["cache-provider"] = value; }
        }

        [ConfigurationProperty("minimize-puts", DefaultValue = false)]
        public bool MinimizePuts
        {
            get { return (bool)this["minimize-puts"]; }
            set { this["minimize-puts"] = value; }
        }
    }
}