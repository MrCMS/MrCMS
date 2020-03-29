using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Tasks;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
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

        public Task<IPagedList<QueuedTask>> GetQueuedTasks(QueuedTaskSearchQuery searchQuery)
        {
            return _repository.Query()
                .OrderByDescending(task => task.CreatedOn)
                .ToPagedListAsync(searchQuery.Page, 10);
        }

        public async Task Update(TaskUpdateData info)
        {
            await _taskSettingManager.Update(info.Type, info.Enabled, info.FrequencyInSeconds);
        }

        public async Task Reset(TaskUpdateData info)
        {
            await _taskSettingManager.Reset(info.Type, true);
        }
    }
}