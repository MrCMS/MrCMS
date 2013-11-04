using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class ScheduledTaskManager : IScheduledTaskManager
    {
        private readonly ISession _session;

        public ScheduledTaskManager(ISession session)
        {
            _session = session;
        }

        public IEnumerable<ScheduledTask> GetDueTasks()
        {
            var scheduledTasks =
                _session.QueryOver<ScheduledTask>().Cacheable().List()
                    .Where(
                        task => task.LastRun < CurrentRequestData.Now.AddMinutes(-task.EveryXMinutes) || task.LastRun == null)
                    .ToList();
            _session.Transact(session =>
            {
                foreach (var task in scheduledTasks)
                {
                    task.LastRun = CurrentRequestData.Now;
                    _session.Update(task);
                }
            });
            return scheduledTasks;
        }

        public BackgroundTask GetTask(ScheduledTask scheduledTask)
        {
            var taskType = TypeHelper.GetAllTypes().FirstOrDefault(type => type.FullName == scheduledTask.Type);
            var backgroundTask = MrCMSApplication.Get(taskType) as BackgroundTask;
            backgroundTask.Site = scheduledTask.Site;
            return backgroundTask;
        }

        public List<ScheduledTask> GetAllTasks()
        {
            return
                _session.QueryOver<ScheduledTask>().Cacheable().List().ToList();
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