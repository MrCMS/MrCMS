using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Shortcodes.Forms;
using MrCMS.Website.Caching;
using NHibernate;

namespace MrCMS.Settings
{
    public class SiteSettingsOptionGenerator
    {
        public virtual List<SelectListItem> GetErrorPageOptions(ISession session, int pageId)
        {
            var list = session.QueryOver<Webpage>().Where(webpage => webpage.Parent == null).Cacheable().List();
            return
                list.Where(page => page.Published)
                         .BuildSelectItemList(
                             page => page.Name,
                             page => page.Id.ToString(CultureInfo.InvariantCulture),
                             page => page.Id == pageId, (string)null);
        }

        public virtual List<SelectListItem> GetMediaCategoryOptions(ISession session, int? categoryId)
        {
            var list =
                session.QueryOver<MediaCategory>()
                       .Where(category => category.Parent == null && !category.HideInAdminNav)
                       .Cacheable()
                       .List();
            return
                list.BuildSelectItemList(
                    category => category.Name,
                    category => category.Id.ToString(CultureInfo.InvariantCulture),
                    category => category.Id == categoryId, (string)null);
        }

        public virtual List<SelectListItem> GetLayoutOptions(ISession session, int? selectedLayoutId)
        {
            return session.QueryOver<Layout>()
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
            return Directory.GetDirectories(applicationPhysicalPath, "*")
                            .Select(x => new DirectoryInfo(x.ToString()).Name)
                            .ToList()
                            .BuildSelectItemList(s => s, s => s, s => s == themeName, emptyItem: null);
        }

        public virtual List<SelectListItem> GetUiCultures(string uiCulture)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(info => info.DisplayName)
                              .BuildSelectItemList(info => info.DisplayName, info => info.Name,
                                                   info => info.Name == uiCulture, emptyItem: null);
        }

        public virtual List<SelectListItem> GetTimeZones(string timeZone)
        {
            return TimeZoneInfo.GetSystemTimeZones().BuildSelectItemList(info => info.DisplayName,
                                                                  info => info.Id, info => info.Id == timeZone,
                                                                  emptyItem: null);
        }

        public virtual List<SelectListItem> GetFormRendererOptions(FormRenderingType defaultFormRendererType)
        {
            return Enum.GetValues(typeof (FormRenderingType)).Cast<FormRenderingType>()
                .BuildSelectItemList(type => type.ToString().BreakUpString(),
                    type => type.ToString(),
                    type => type == defaultFormRendererType,
                    emptyItem: null);
        }

        public virtual List<SelectListItem> GetCacheExpiryTypeOptions()
        {
            return Enum.GetValues(typeof (CacheExpiryType)).Cast<CacheExpiryType>()
                .BuildSelectItemList(type => type.ToString().BreakUpString(),
                    type => type.ToString(),
                    emptyItem: null);
        }
    }
}