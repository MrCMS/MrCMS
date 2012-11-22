using System.Linq;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class RootNavigationWidget : Widget
    {
        public override bool HasProperties
        {
            get { return false; }
        }
        public override object GetModel(ISession session)
        {
            var navigationRecords =
                session.QueryOver<Webpage>().Where(
                    webpage => webpage.Parent == null && webpage.Published && webpage.RevealInNavigation)
                    .OrderBy(webpage => webpage.DisplayOrder).Asc.List()
                    .Select(webpage => new NavigationRecord
                                           {
                                               Text = webpage.Name,
                                               Url = "/" + webpage.LiveUrlSegment
                                           }).ToList();

            if (navigationRecords.Any())
            {
                var navigationRecord = navigationRecords.First();
                navigationRecord.Url = "/";
            }
            
            return new NavigationList(navigationRecords.ToList());
        }
    }
}