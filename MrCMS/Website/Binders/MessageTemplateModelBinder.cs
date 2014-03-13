using MrCMS.Services;
using NHibernate;
using Ninject;

namespace MrCMS.Website.Binders
{
    public abstract class MessageTemplateModelBinder : MrCMSDefaultModelBinder
    {
        private IMessageTemplateService _messageTemplateService;

        protected MessageTemplateModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        protected IMessageTemplateService MessageTemplateService
        {
            get { return _messageTemplateService = _messageTemplateService ?? Get<IMessageTemplateService>(); }
        }
    }
}