using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class SetAdminViewDataExtensions
    {
        public static void SetAdminViewData<T>(this T webpage, ViewDataDictionary viewDataDictionary) where T : Webpage
        {
            MrCMSApplication.Get<ISetWebpageAdminViewData>().SetViewData(webpage, viewDataDictionary);
        }

        public static void SetViewData<T>(this T widget, ViewDataDictionary viewDataDictionary) where T : Widget
        {
            MrCMSApplication.Get<ISetWidgetAdminViewData>().SetViewData(widget, viewDataDictionary);
        }
    }
}