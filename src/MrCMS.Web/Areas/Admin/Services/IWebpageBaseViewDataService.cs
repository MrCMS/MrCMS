using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWebpageBaseViewDataService
    {
        Task SetAddPageViewData(ViewDataDictionary viewData, Webpage parent);
        Task SetEditPageViewData(ViewDataDictionary viewData, Webpage page);
    }
}