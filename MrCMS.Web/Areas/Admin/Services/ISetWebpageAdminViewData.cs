using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ISetWebpageAdminViewData
    {
        void SetViewData<T>(T webpage, ViewDataDictionary viewData) where T : Webpage;
    }
}