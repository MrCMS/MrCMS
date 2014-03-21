using System;
using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Website.Binders
{
    public class AddWidgetModelBinder : MrCMSDefaultModelBinder
    {
        public AddWidgetModelBinder(IKernel kernel) : base(kernel)
        {
        }

        private static Type GetTypeByName(ControllerContext controllerContext)
        {
            return WidgetHelper.GetTypeByName(GetValueFromContext(controllerContext, "WidgetType"));
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(controllerContext);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var widget = base.BindModel(controllerContext, bindingContext) as Widget;

            if (widget != null)
            {
                if (widget.LayoutArea != null)
                    widget.LayoutArea.AddWidget(widget);
                if (widget.Webpage != null)
                    widget.Webpage.Widgets.Add(widget);
                if (GetValueFromContext(controllerContext, "AddType") == "global")
                {
                    widget.Webpage = null;
                }
            }
            return widget;
        }

    }
}