using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Admin.Models;

using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class TaskAdminService : ITaskAdminService
    {
        private readonly IGlobalRepository<QueuedTask> _repository;
        private readonly ITaskSettingManager _taskSettingManager;

        public TaskAdminService(IGlobalRepository<QueuedTask> repository, ITaskSettingManager taskSettingManager)
        {
            _repository = repository;
            _taskSettingManager = taskSettingManager;
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
                    FrequencyInSeconds = info.FrequencyInSeconds,
                    Name = info.Name
                };
        }

        public IPagedList<QueuedTask> GetQueuedTasks(QueuedTaskSearchQuery searchQuery)
        {
            return _repository.Query()
                .OrderByDescending(task => task.CreatedOn)
                .ToPagedList(searchQuery.Page);
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