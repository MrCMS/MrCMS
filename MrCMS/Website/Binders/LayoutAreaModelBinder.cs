using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using NHibernate;

namespace MrCMS.Website.Binders
{
    public class LayoutAreaModelBinder : MrCMSDefaultModelBinder
    {
        public LayoutAreaModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = typeof(LayoutArea);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            return base.BindModel(controllerContext, bindingContext);
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var value = controllerContext.HttpContext.Request["Id"] ?? controllerContext.RouteData.Values["id"];
            return Session.Get<LayoutArea>(Convert.ToInt32(value));
        }
    }
}