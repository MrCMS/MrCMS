using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Models;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
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

        public async Task<ActionResult> Show(int id)
        {
            var queuedMessage = await _messageQueueAdminService.GetMessage(id);
            return View(queuedMessage);
        }

        public async Task<ContentResult> GetBody(int id)
        {
            QueuedMessage queuedMessage = await _messageQueueAdminService.GetMessage(id);
            if (queuedMessage.IsHtml)
                return Content(queuedMessage.Body, "text/html");
            return Content(queuedMessage.Body);
        }
    }
}