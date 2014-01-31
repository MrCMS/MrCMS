using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Paging;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskManager : ITaskManager
    {
        private readonly ISession _session;

        public TaskManager(ISession session)
        {
            _session = session;
        }

        public List<ScheduledTask> GetAllScheduledTasks()
        {
            return _session.QueryOver<ScheduledTask>().Cacheable().List().ToList();
        }

        public IPagedList<QueuedTask> GetQueuedTask(QueuedTaskSearchQuery searchQuery)
        {
            var queryOver = _session.QueryOver<QueuedTask>();

            return queryOver.OrderBy(task => task.CreatedOn).Desc.Paged(searchQuery.Page);
        }

        public void Add(ScheduledTask scheduledTask)
        {
            _session.Transact(session => session.Save(scheduledTask));
        }

        public void Update(ScheduledTask scheduledTask)
        {
            _session.Transact(session => session.Update(scheduledTask));
        }

        public void Delete(ScheduledTask scheduledTask)
        {
            _session.Transact(session => session.Delete(scheduledTask));
        }
    }
}