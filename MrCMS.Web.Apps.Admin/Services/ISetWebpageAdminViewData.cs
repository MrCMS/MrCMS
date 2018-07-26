using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ISetWebpageAdminViewData
    {
        void SetViewData<T>(T webpage, ViewDataDictionary viewData) where T : Webpage;
    }
}