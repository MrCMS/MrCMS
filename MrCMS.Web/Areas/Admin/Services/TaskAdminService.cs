using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class TaskAdminService : ITaskAdminService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public TaskAdminService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public List<ScheduledTask> GetAllScheduledTasks()
        {
            return _session.QueryOver<ScheduledTask>()
                .Where(task => task.Site.Id == _site.Id)
                .Cacheable()
                .List()
                .ToList();
        }

        public IPagedList<QueuedTask> GetQueuedTasks(QueuedTaskSearchQuery searchQuery)
        {
            return _session.QueryOver<QueuedTask>()
                .Where(task => task.Site.Id == _site.Id)
                .OrderBy(task => task.CreatedOn)
                .Desc.Paged(searchQuery.Page);
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