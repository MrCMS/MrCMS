using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Tasks.Entities;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class TaskAdminService : ITaskAdminService
    {
        private readonly ISession _session;
        private readonly ITaskSettingManager _taskSettingManager;

        public TaskAdminService(ISession session,ITaskSettingManager taskSettingManager)
        {
            _session = session;
            _taskSettingManager = taskSettingManager;
        }

        public List<TaskInfo> GetAllScheduledTasks()
        {
            return _taskSettingManager.GetInfo().OrderBy(x => x.Name).ToList();
        }

        public TaskUpdateData GetTaskUpdateData(string type)
        {
            var info = GetAllScheduledTasks().FirstOrDefault(x => x.TypeName == type);

            return info == null
                ? null
                : new TaskUpdateData
                {
                    Enabled = info.Enabled,
                    TypeName = info.TypeName,
                    FrequencyInSeconds = info.FrequencyInSeconds,
                    Name = info.Name
                };
        }

        public IPagedList<QueuedTask> GetQueuedTasks(QueuedTaskSearchQuery searchQuery)
        {
            using (new SiteFilterDisabler(_session))
            {
                return _session.QueryOver<QueuedTask>()
                    .OrderBy(task => task.CreatedOn).Desc
                    .Paged(searchQuery.Page);
            }
        }

        public void Update(TaskUpdateData info)
        {
            _taskSettingManager.Update(info.Type, info.Enabled, info.FrequencyInSeconds);
        }

        public void Reset(TaskUpdateData info)
        {
            _taskSettingManager.Reset(info.Type, true);
        }
    }
}