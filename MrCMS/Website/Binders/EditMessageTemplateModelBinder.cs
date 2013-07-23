using System.Web.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Website.Binders
{
    public class EditMessageTemplateModelBinder : MessageTemplateModelBinder
    {
        public EditMessageTemplateModelBinder(ISession session, IMessageTemplateService messageTemplateService) : base(session, messageTemplateService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return base.BindModel(controllerContext, bindingContext) as MessageTemplate;
        }
    }
}