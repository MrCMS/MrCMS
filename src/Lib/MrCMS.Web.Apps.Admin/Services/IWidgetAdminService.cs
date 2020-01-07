using System.Threading.Tasks;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
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