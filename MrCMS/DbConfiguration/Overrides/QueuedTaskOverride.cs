using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Tasks;

namespace MrCMS.DbConfiguration.Overrides
{
    public class QueuedTaskOverride : IAutoMappingOverride<QueuedTask>
    {
        public void Override(AutoMapping<QueuedTask> mapping)
        {
            mapping.Map(task => task.Data).MakeVarCharMax();
        }
    }
}