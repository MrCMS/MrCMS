using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MrCMS.DbConfiguration.Filters;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Conventions
{
    public class PublishedFilterConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            if (instance.EntityType.IsSubclassOf(typeof(Webpage)))
            {
                instance.ApplyFilter<PublishedFilter>();
            }
        }
    }
}