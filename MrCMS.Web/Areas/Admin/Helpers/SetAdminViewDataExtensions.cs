using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class SetAdminViewDataExtensions
    {
        public static void SetAdminViewData<T>(this T webpage, ViewDataDictionary viewDataDictionary) where T : Webpage
        {
            MrCMSApplication.Get<ISetAdminViewData>().SetViewData(webpage, viewDataDictionary);
        }
    }
}