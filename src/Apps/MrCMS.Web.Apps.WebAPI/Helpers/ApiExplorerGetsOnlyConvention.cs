using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using MrCMS.Web.Apps.WebApi.Api;
using Swashbuckle.AspNetCore.Swagger;

namespace MrCMS.Web.Apps.WebApi.Helpers
{
    public class ApiExplorerGetsOnlyConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            action.ApiExplorer.IsVisible = action.Attributes.OfType<MrCMSWebApiAttribute>().Any();
        }
    }

    public class ApiExplorerMrCMSWebApiOnlyConvention : IControllerModelConvention
    {
        private bool showAdminController = false;
        public void Apply(ControllerModel controller)
        {
            controller.ApiExplorer.IsVisible = controller.Attributes.OfType<MrCMSWebApiAttribute>().Any();
            if (showAdminController)
            {
                controller.ApiExplorer.IsVisible = controller.Attributes.OfType<MrCMSAdminWebApiAttribute>().Any();
            }
        }
    }
}