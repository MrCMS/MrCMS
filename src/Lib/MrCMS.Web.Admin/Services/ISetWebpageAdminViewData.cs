using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Services
{
    public interface ISetWebpageAdminViewData
    {
        Task SetViewData<T>(ViewDataDictionary viewData, T webpage) where T : Webpage;
        Task SetViewDataForAdd(ViewDataDictionary viewData, string type);
    }
}