using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHibernate;

namespace MrCMS.Settings
{
    public interface ISiteSettingsOptionGenerator
    {
        List<SelectListItem> GetErrorPageOptions(int pageId);
        List<SelectListItem> GetMediaCategoryOptions(int? categoryId);
        List<SelectListItem> GetLayoutOptions(int? selectedLayoutId);
        List<SelectListItem> GetThemeNames(string themeName);
        List<SelectListItem> GetUiCultures(string uiCulture);
        List<SelectListItem> GetTimeZones(string timeZone);
        List<SelectListItem> GetFormRendererOptions(FormRenderingType defaultFormRendererType);
        List<SelectListItem> GetCacheExpiryTypeOptions();
    }
}