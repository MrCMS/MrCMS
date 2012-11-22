using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;

namespace MrCMS.AddOns.Pages.Portfolio
{
    [MrCMSMapClass]
    public class PortfolioImage : BaseEntity
    {
        public virtual PortfolioItem PortfolioItem { get; set; }
        public virtual bool IsPrimaryImage { get; set; }

        public virtual string Title { get; set; }

        public virtual string Image { get; set; }
    }
}