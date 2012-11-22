using System.Linq;
using MrCMS.AddOns.Pages.Portfolio;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class PortfolioCategories : Widget
    {
        public virtual PortfolioContainer Container { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            if (Container != null)
                return Container;

            return session.QueryOver<PortfolioContainer>().Cacheable().List().FirstOrDefault();
        }
    }
}