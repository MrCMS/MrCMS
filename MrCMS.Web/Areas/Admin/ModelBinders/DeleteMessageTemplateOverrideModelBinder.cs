using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Messages;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class DeleteMessageTemplateOverrideModelBinder : MessageTemplateModelBinder
    {
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly Site _site;

        public DeleteMessageTemplateOverrideModelBinder(IMessageTemplateProvider messageTemplateProvider, Site site,
            IKernel kernel)
            : base(kernel)
        {
            _messageTemplateProvider = messageTemplateProvider;
            _site = site;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type type = GetTypeByName(controllerContext);

            return _messageTemplateProvider.GetAllMessageTemplates(_site)
                .FirstOrDefault(template => template.SiteId == _site.Id && template.GetType() == type);
        }

        private static Type GetTypeByName(ControllerContext controllerContext)
        {
            string valueFromContext = GetValueFromContext(controllerContext, "TemplateType");
            return TypeHelper.GetTypeByName(valueFromContext);
        }
    }
}