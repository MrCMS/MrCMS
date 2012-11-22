using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class ThisRootPageSubnavWidget : Widget
    {
        public override object GetModel(ISession session)
        {
            var currentPage = MrCMSApplication.CurrentPage;

            var rootParent = currentPage.RootPage;

            return rootParent;
        }
    }
}