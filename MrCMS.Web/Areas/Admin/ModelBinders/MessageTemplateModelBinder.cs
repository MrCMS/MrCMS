using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public abstract class MessageTemplateModelBinder : MrCMSDefaultModelBinder
    {
        private IMessageTemplateAdminService _messageTemplateAdminService;

        protected MessageTemplateModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        protected IMessageTemplateAdminService MessageTemplateAdminService
        {
            get { return _messageTemplateAdminService = _messageTemplateAdminService ?? Get<IMessageTemplateAdminService>(); }
        }
    }
}