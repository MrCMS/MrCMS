using Microsoft.AspNetCore.Mvc;
using MrCMS.Messages;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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