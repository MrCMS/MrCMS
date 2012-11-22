using MrCMS.Entities.Documents;
using MrCMS.Helpers;

namespace MrCMS.Entities.Settings
{
    public class SettingValue : BaseEntity
    {
        public virtual string SettingKey { get; set; }
        public virtual string Value { get; set; }

        public virtual T ValueAs<T>()
        {
            return Value.ConvertTo<T>();
        }
    }
}