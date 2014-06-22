using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Settings
{
    public class LegacySettingsProvider : ILegacySettingsProvider
    {
        private readonly ISession _session;

        public LegacySettingsProvider(ISession session)
        {
            _session = session;
        }

        public void ApplyLegacySettings<TSettings>(TSettings settings, int siteId) where TSettings : SiteSettingsBase
        {
            IList<Setting> legacySettings;
            string typeNameStart = typeof(TSettings).FullName + ".";
            using (new SiteFilterDisabler(_session))
            {
                legacySettings = _session.QueryOver<Setting>()
                    .Where(setting => setting.Name.IsInsensitiveLike(typeNameStart, MatchMode.Start) && setting.Site.Id == siteId)
                    .Cacheable()
                    .List();
            }

            // get properties we can write to
            var properties = from prop in typeof(TSettings).GetProperties()
                where prop.CanWrite && prop.CanRead
                let setting =
                    GetValue(string.Format("{0}.{1}", typeof(TSettings).FullName, prop.Name), legacySettings)
                where setting != null
                where prop.PropertyType.GetCustomTypeConverter().CanConvertFrom(typeof(string))
                where prop.PropertyType.GetCustomTypeConverter().IsValid(setting)
                let value = prop.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(setting)
                select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(settings, p.value, null));
        }

        private string GetValue(string key, IList<Setting> settings)
        {
            Setting setting =
                settings.FirstOrDefault(s => s.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            return setting != null
                ? setting.Value
                : null;
        }
    }
}