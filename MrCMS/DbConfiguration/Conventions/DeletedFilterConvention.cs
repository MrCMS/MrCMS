using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MrCMS.DbConfiguration.Filters;

namespace MrCMS.DbConfiguration.Conventions
{
    public class DeletedFilterConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.ApplyFilter<NotDeletedFilter>();
        }
    }
}