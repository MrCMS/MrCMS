using System.Web.Mvc;
using MrCMS.Messages;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MessageTemplateTokensController : MrCMSAdminController
    {
        private readonly IMessageTemplateTokensAdminService _messageTemplateTokensAdminService;

        public MessageTemplateTokensController(IMessageTemplateTokensAdminService messageTemplateTokensAdminService)
        {
            _messageTemplateTokensAdminService = messageTemplateTokensAdminService;
        }

        public PartialViewResult Tokens(MessageTemplate messageTemplate)
        {
            return PartialView(_messageTemplateTokensAdminService.GetTokens(messageTemplate));
        }
    }
}