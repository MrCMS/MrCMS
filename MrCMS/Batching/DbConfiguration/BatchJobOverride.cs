using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Batching.Entities;
using MrCMS.DbConfiguration;

namespace MrCMS.Batching.DbConfiguration
{
    public class BatchJobOverride : IAutoMappingOverride<BatchJob>
    {
        public void Override(AutoMapping<BatchJob> mapping)
        {
            mapping.Map(job => job.Data).MakeVarCharMax();
        }
    }
}