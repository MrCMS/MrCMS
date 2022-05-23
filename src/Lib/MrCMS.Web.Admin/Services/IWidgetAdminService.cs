using System.Threading.Tasks;
using MrCMS.Entities.Widget;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IWidgetAdminService
    {
        object GetAdditionalPropertyModel(string type);
        Task<Widget> AddWidget(AddWidgetModel model, object additionalPropertyModel);
        Task<UpdateWidgetModel> GetEditModel(int id);
        Task<Widget> GetWidget(int id);
        Task<object> GetAdditionalPropertyModel(int id);
        Task<Widget> UpdateWidget(UpdateWidgetModel model, object additionalPropertyModel);
        Task<Widget> DeleteWidget(int id);
        object GetUpdateAdditionalPropertyModel(string type);
        Task<object> GetUpdateAdditionalPropertyModel(int id);
    }

}