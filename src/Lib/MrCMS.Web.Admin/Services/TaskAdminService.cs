using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Scheduling;
using MrCMS.Tasks;
using MrCMS.Web.Admin.Models;
using NHibernate;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class TaskAdminService : ITaskAdminService
    {
        private readonly ISession _session;
        private readonly ITaskSettingManager _taskSettingManager;
        private readonly IAdHocJobScheduler _adHocJobScheduler;

        public TaskAdminService(ISession session, ITaskSettingManager taskSettingManager,
            IAdHocJobScheduler adHocJobScheduler)
        {
            _session = session;
            _taskSettingManager = taskSettingManager;
            _adHocJobScheduler = adHocJobScheduler;
        }

        public async Task<List<TaskInfo>> GetAllScheduledTasks()
        {
            var info = await _taskSettingManager.GetInfo();
            return info.OrderBy(x => x.Name).ToList();
        }

        public async Task<TaskUpdateData> GetTaskUpdateData(string type)
        {
            var allScheduledTasks = await GetAllScheduledTasks();
            var info = allScheduledTasks.FirstOrDefault(x => x.TypeName == type);

            return info == null
                ? null
                : new TaskUpdateData
                {
                    Enabled = info.Enabled,
                    TypeName = info.TypeName,
                    CronSchedule = info.CronSchedule,
                    Name = info.Name
                };
        }

        public async Task<IPagedList<QueuedTask>> GetQueuedTasks(QueuedTaskSearchQuery searchQuery)
        {
            using (new SiteFilterDisabler(_session))
            {
                return await _session.QueryOver<QueuedTask>()
                    .OrderBy(task => task.CreatedOn).Desc
                    .PagedAsync(searchQuery.Page);
            }
        }

        public async Task<bool> Update(TaskUpdateData info)
        {
            return await _taskSettingManager.Update(info.Type, info.Enabled, info.CronSchedule ?? string.Empty);
        }


        public async Task Execute(TaskUpdateData taskInfo)
        {
            await _adHocJobScheduler.Schedule(taskInfo.Type);
        }
    }
}