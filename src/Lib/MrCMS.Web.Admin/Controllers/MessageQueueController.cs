using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class MessageQueueController : MrCMSAdminController
    {
        private readonly IMessageQueueAdminService _messageQueueAdminService;

        public MessageQueueController(IMessageQueueAdminService messageQueueAdminService)
        {
            _messageQueueAdminService = messageQueueAdminService;
        }

        public ViewResult Index(MessageQueueQuery searchQuery)
        {
            ViewData["data"] = _messageQueueAdminService.GetMessages(searchQuery);
            return View(searchQuery);
        }

        public ActionResult Show(QueuedMessage queuedMessage)
        {
            return View(queuedMessage);
        }

        public ContentResult GetBody(int id)
        {
            QueuedMessage queuedMessage = _messageQueueAdminService.GetMessageBody(id);
            if (queuedMessage.IsHtml)
                return Content(queuedMessage.Body, "text/html");
            return Content(queuedMessage.Body);
        }
    }
}