using MrCMS.Entities.Widget;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IWidgetAdminService
    {
        object GetAdditionalPropertyModel(string type);
        Widget AddWidget(AddWidgetModel model, object additionalPropertyModel);
        UpdateWidgetModel GetEditModel(int id);
        Widget GetWidget(int id);
        object GetAdditionalPropertyModel(int id);
        Widget UpdateWidget(UpdateWidgetModel model, object additionalPropertyModel);
        Widget DeleteWidget(int id);
    }

}