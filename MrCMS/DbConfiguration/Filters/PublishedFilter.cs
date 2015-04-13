using FluentNHibernate.Mapping;

namespace MrCMS.DbConfiguration.Filters
{
    public class PublishedFilter : FilterDefinition
    {
        public const string FilterName = "PublishedFilter";

        public PublishedFilter()
        {
            WithName(FilterName).WithCondition("(Published = 'False' or Published = 0 or Published is null)");
        }
    }
}