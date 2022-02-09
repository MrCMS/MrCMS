using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Settings;

namespace MrCMS.DbConfiguration.Overrides
{
    public class SettingOverride : IAutoMappingOverride<Setting>
    {
        public void Override(AutoMapping<Setting> mapping)
        {
            mapping.Map(setting => setting.Value).CustomType<VarcharMax>().Length(4001);

            mapping.Map(setting => setting.SettingType).Length(120);
            mapping.Map(setting => setting.PropertyName).Length(50);
        }
    }
}