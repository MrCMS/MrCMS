using System.Web.Mvc;
using MrCMS.Tasks;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class TaskController : MrCMSAdminController
    {
        private readonly ITaskManager _taskManager;

        public TaskController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public ViewResult Index(QueuedTaskSearchQuery searchQuery)
        {
            ViewData["scheduled-tasks"] = _taskManager.GetAllScheduledTasks();
            ViewData["tasks"] = _taskManager.GetQueuedTasks(searchQuery);
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
            _taskManager.Add(scheduledTask);

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
            _taskManager.Update(scheduledTask);

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
            _taskManager.Delete(scheduledTask);
            return RedirectToAction("Index");
        }
    }
}