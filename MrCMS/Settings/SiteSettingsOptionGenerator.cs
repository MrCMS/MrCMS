using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Settings
{
    public class SiteSettingsOptionGenerator
    {
        public virtual List<SelectListItem> GetErrorPageOptions(ISession session, Site site, int pageId)
        {
            var list = session.QueryOver<Webpage>().Where(webpage => webpage.Site == site).Cacheable().List();
            return
                list.Where(page => page.Published)
                         .BuildSelectItemList(
                             page => page.Name,
                             page => page.Id.ToString(CultureInfo.InvariantCulture),
                             page => page.Id == pageId, (string) null);
        }

        public virtual List<SelectListItem> GetLayoutOptions(ISession session, Site site, int? selectedLayoutId)
        {
            return session.QueryOver<Layout>()
                          .Where(layout => layout.Site == site)
                          .Cacheable()
                          .List()
                          .BuildSelectItemList(
                              layout => layout.Name,
                              layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                              layout => layout.Id == selectedLayoutId,
                              emptyItem: null);
        }

        public virtual List<SelectListItem> GetThemeNames(string themeName)
        {
            var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath + "\\Themes\\";
            return Directory.GetDirectories(applicationPhysicalPath , "*")
                            .Select(x => new DirectoryInfo(x.ToString()).Name)
                            .ToList()
                            .BuildSelectItemList(s => s, s => s, s => s == themeName, emptyItem: null);
        }
    }
}