using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Website.Caching;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MrCMS.Data;

namespace MrCMS.Settings
{
    public class SiteSettingsOptionGenerator : ISiteSettingsOptionGenerator
    {
        private readonly IDataReader _dataReader;
        private readonly MrCMSAppContext _appContext;

        public SiteSettingsOptionGenerator(IDataReader dataReader, MrCMSAppContext appContext)
        {
            _dataReader = dataReader;
            _appContext = appContext;
        }

        public virtual List<SelectListItem> GetErrorPageOptions(int pageId)
        {
            var list = _dataReader.Readonly<Webpage>().Where(webpage => webpage.Parent == null).ToList();
            return
                list.Where(page => page.Published)
                         .BuildSelectItemList(
                             page => page.Name,
                             page => page.Id.ToString(CultureInfo.InvariantCulture),
                             page => page.Id == pageId, (string)null);
        }

        public virtual List<SelectListItem> GetMediaCategoryOptions(int? categoryId)
        {
            var list =
                _dataReader.Readonly<MediaCategory>()
                       .Where(category => category.Parent == null && !category.HideInAdminNav)
                       .ToList();
            return
                list.BuildSelectItemList(
                    category => category.Name,
                    category => category.Id.ToString(CultureInfo.InvariantCulture),
                    category => category.Id == categoryId, (string)null);
        }

        public virtual List<SelectListItem> GetLayoutOptions(int? selectedLayoutId)
        {
            return _dataReader.Readonly<Layout>()
                .ToList()
                .BuildSelectItemList(
                    layout => layout.Name,
                    layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                    layout => layout.Id == selectedLayoutId,
                    emptyItem: null);
        }

        public virtual List<SelectListItem> GetThemeNames(string themeName)
        {
            return _appContext.Themes.BuildSelectItemList(theme => theme.Name,
                selected: theme => theme.Name == themeName,
                emptyItem: SelectListItemHelper.EmptyItem("None", ""));
        }

        public virtual List<SelectListItem> GetUiCultures(string uiCulture)
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).OrderBy(info => info.DisplayName)
                              .BuildSelectItemList(info => info.DisplayName, info => info.Name,
                                                   info => info.Name == uiCulture, emptyItem: null);
        }

        public virtual List<SelectListItem> GetTimeZones(string timeZone)
        {
            return TimeZones.Zones.BuildSelectItemList(info => info.DisplayName,
                                                                  info => info.ToSerializedString(), info => info.ToSerializedString() == timeZone,
                                                                  emptyItem: null);
        }

        public virtual List<SelectListItem> GetFormRendererOptions(FormRenderingType defaultFormRendererType)
        {
            return Enum.GetValues(typeof(FormRenderingType)).Cast<FormRenderingType>()
                .BuildSelectItemList(type => type.ToString().BreakUpString(),
                    type => type.ToString(),
                    type => type == defaultFormRendererType,
                    emptyItem: null);
        }

        public virtual List<SelectListItem> GetCacheExpiryTypeOptions()
        {
            return Enum.GetValues(typeof(CacheExpiryType)).Cast<CacheExpiryType>()
                .BuildSelectItemList(type => type.ToString().BreakUpString(),
                    type => type.ToString(),
                    emptyItem: null);
        }
    }
}