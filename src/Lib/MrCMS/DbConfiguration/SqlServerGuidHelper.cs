using System.Linq;
using NHibernate.Mapping;

namespace MrCMS.DbConfiguration
{
    public class SqlServerGuidHelper
    {
        public static void SetGuidToUniqueWithDefaultValue(NHibernate.Cfg.Configuration config)
        {
            foreach (PersistentClass @class in config.ClassMappings)
            {
                RootClass rootClass = @class.RootClazz;
                Table table = rootClass.Table;
                foreach (Column column in table.ColumnIterator.Where(column => column.Name == "Guid"))
                {
                    column.IsUnique = true;
                    column.DefaultValue = "newid()";
                }
            }
        }
    }
}