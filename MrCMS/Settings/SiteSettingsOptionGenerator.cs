using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Settings
{
    public class SiteSettingsOptionGenerator
    {
        public List<SelectListItem> GetErrorPageOptions(ISession session, Site site, int pageId)
        {
            var queryOver = session.QueryOver<Webpage>().Where(webpage => webpage.Site == site).Cacheable().List();
            return
                queryOver.Where(page => page.Published)
                         .BuildSelectItemList(
                             page => page.Name,
                             page => page.Id.ToString(CultureInfo.InvariantCulture),
                             page => page.Id == pageId, (string) null);
        }

        public List<SelectListItem> GetLayoutOptions(ISession session, Site site, int? selectedLayoutId, bool includeDefault = false)
        {
            var selectListItem = SelectListItemHelper.EmptyItem("Default Layout");
            selectListItem.Selected = !selectedLayoutId.HasValue;
            return session.QueryOver<Layout>()
                          .Where(layout => layout.Site == site)
                          .Cacheable()
                          .List()
                          .BuildSelectItemList(
                              layout => layout.Name,
                              layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                              layout => layout.Id == selectedLayoutId,
                              includeDefault ? selectListItem : null);
        }
    }
}