using System;
using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using System.Linq;

namespace MrCMS.Tasks
{
    public interface IScheduledTaskManager
    {
        IEnumerable<ScheduledTask> GetDueTasks();

        BackgroundTask GetTask(ScheduledTask scheduledTask);
        List<ScheduledTask> GetAllTasks();
        void Add(ScheduledTask scheduledTask);
        void Update(ScheduledTask scheduledTask);
        void Delete(ScheduledTask scheduledTask);
    }

    public class ScheduledTaskManager : IScheduledTaskManager
    {
        private readonly ISession _session;
        private readonly CurrentSite _currentSite;

        public ScheduledTaskManager(ISession session, CurrentSite currentSite)
        {
            _session = session;
            _currentSite = currentSite;
        }

        public IEnumerable<ScheduledTask> GetDueTasks()
        {
            var scheduledTasks = GetAllTasks().Where(task => task.LastRun < DateTime.UtcNow.AddMinutes(-task.EveryXMinutes) || task.LastRun == null);
            _session.Transact(session =>
                                  {
                                      foreach (var task in scheduledTasks)
                                      {
                                          task.LastRun = DateTime.UtcNow;
                                          _session.Update(task);
                                      }
                                  });
            return scheduledTasks;
        }

        public BackgroundTask GetTask(ScheduledTask scheduledTask)
        {
            var taskType = TypeHelper.GetAllTypes().FirstOrDefault(type => type.FullName == scheduledTask.Type);
            return MrCMSApplication.Get(taskType) as BackgroundTask;
        }

        public List<ScheduledTask> GetAllTasks()
        {
            return
                _session.QueryOver<ScheduledTask>().Where(task => task.Site == _currentSite).Cacheable().List().ToList();
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