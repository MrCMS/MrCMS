using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Admin.Services
{
    public interface ISetWidgetAdminViewData
    {
        Task SetViewData<T>(ViewDataDictionary viewData, T widget) where T : Widget;
        Task SetViewDataForAdd(ViewDataDictionary viewData, string type);
    }
}