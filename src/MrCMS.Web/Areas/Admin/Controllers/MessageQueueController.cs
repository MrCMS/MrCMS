using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MessageQueueController : MrCMSAdminController
    {
        private readonly IMessageQueueAdminService _messageQueueAdminService;

        public MessageQueueController(IMessageQueueAdminService messageQueueAdminService)
        {
            _messageQueueAdminService = messageQueueAdminService;
        }

        public async Task<ViewResult> Index(MessageQueueQuery searchQuery)
        {
            ViewData["data"] = await _messageQueueAdminService.GetMessages(searchQuery);
            return View(searchQuery);
        }

        public ActionResult Show(QueuedMessage queuedMessage)
        {
            return View(queuedMessage);
        }

        public async Task<ContentResult> GetBody(int id)
        {
            QueuedMessage queuedMessage = await _messageQueueAdminService.GetMessageBody(id);
            if (queuedMessage.IsHtml)
                return Content(queuedMessage.Body, "text/html");
            return Content(queuedMessage.Body);
        }
    }
}