using MrCMS.Services;
using NHibernate;

namespace MrCMS.Website.Binders
{
    public abstract class MessageTemplateModelBinder : MrCMSDefaultModelBinder
    {
        protected readonly IMessageTemplateService MessageTemplateService;

        protected MessageTemplateModelBinder(ISession session, IMessageTemplateService messageTemplateService)
            : base(() => session)
        {
            this.MessageTemplateService = messageTemplateService;
        }
    }
}