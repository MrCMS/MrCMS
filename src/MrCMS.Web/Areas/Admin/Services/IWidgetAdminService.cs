using System.Threading.Tasks;
using MrCMS.Entities.Widget;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWidgetAdminService
    {
        object GetAdditionalPropertyModel(string type);
        Task<Widget> AddWidget(AddWidgetModel model, object additionalPropertyModel);
        UpdateWidgetModel GetEditModel(int id);
        Widget GetWidget(int id);
        object GetAdditionalPropertyModel(int id);
        Task<Widget> UpdateWidget(UpdateWidgetModel model, object additionalPropertyModel);
        Task<Widget> DeleteWidget(int id);
    }

}