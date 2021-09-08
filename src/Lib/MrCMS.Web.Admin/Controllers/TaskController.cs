using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Tasks;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class TaskController : MrCMSAdminController
    {
        private readonly ITaskAdminService _taskAdminService;

        public TaskController(ITaskAdminService taskAdminService)
        {
            _taskAdminService = taskAdminService;
        }

        public async Task<ViewResult> Index(QueuedTaskSearchQuery searchQuery)
        {
            ViewData["scheduled-tasks"] = await _taskAdminService.GetAllScheduledTasks();
            ViewData["tasks"] = await _taskAdminService.GetQueuedTasks(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string type)
        {
            var taskInfo = await _taskAdminService.GetTaskUpdateData(type);
            return PartialView(taskInfo);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(TaskUpdateData taskInfo)
        {
            var result = await _taskAdminService.Update(taskInfo);

            if (result)
            {
                TempData.AddSuccessMessage("Updated");
            }
            else
            {
                TempData.AddErrorMessage("Could not update");
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<PartialViewResult> Execute(string type)
        {
            var taskInfo = await _taskAdminService.GetTaskUpdateData(type);
            return PartialView(taskInfo);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Execute(TaskUpdateData taskInfo)
        {
            await _taskAdminService.Execute(taskInfo);
            
            TempData.AddSuccessMessage("Started");

            return RedirectToAction("Index");
        }
    }
}