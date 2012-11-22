using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.AddOns.Pages.Portfolio;

namespace MrCMS.DbConfiguration.Overrides
{
    public class PortfolioPageOverride : IAutoMappingOverride<PortfolioItem>
    {
        public void Override(AutoMapping<PortfolioItem> mapping)
        {
            mapping.HasMany(x => x.Images).Cascade.Delete();
        }
    }
}