using FluentNHibernate.Mapping;

namespace MrCMS.DbConfiguration.Filters
{
    public class NotDeletedFilter : FilterDefinition
    {
        public NotDeletedFilter()
        {
            WithName("NotDeletedFilter").WithCondition("(IsDeleted = 'False' or IsDeleted = 0 or IsDeleted is null)");
        }
    }
}