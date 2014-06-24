using System.Web.Mvc;
using MrCMS.Tasks;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class TaskController : MrCMSAdminController
    {
        private readonly ITaskAdminService _taskAdminService;

        public TaskController(ITaskAdminService taskAdminService)
        {
            _taskAdminService = taskAdminService;
        }

        public ViewResult Index(QueuedTaskSearchQuery searchQuery)
        {
            ViewData["scheduled-tasks"] = _taskAdminService.GetAllScheduledTasks();
            ViewData["tasks"] = _taskAdminService.GetQueuedTasks(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView(new ScheduledTask());
        }

        [HttpPost]
        public RedirectToRouteResult Add(ScheduledTask scheduledTask)
        {
            _taskAdminService.Add(scheduledTask);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(ScheduledTask scheduledTask)
        {
            return PartialView(scheduledTask);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_Post(ScheduledTask scheduledTask)
        {
            _taskAdminService.Update(scheduledTask);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(ScheduledTask scheduledTask)
        {
            return PartialView(scheduledTask);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_Post(ScheduledTask scheduledTask)
        {
            _taskAdminService.Delete(scheduledTask);
            return RedirectToAction("Index");
        }
    }
}