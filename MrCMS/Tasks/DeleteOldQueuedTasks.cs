using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class DeleteOldQueuedTasks : SchedulableTask
    {
        private readonly IStatelessSession _statelessSession;

        public DeleteOldQueuedTasks(IStatelessSession statelessSession)
        {
            _statelessSession = statelessSession;
        }

        public override int Priority { get { return 10; } }

        protected override void OnExecute()
        {
            var logs =
                _statelessSession.QueryOver<QueuedTask>().Where(data => data.CompletedAt <= CurrentRequestData.Now.AddDays(-1)).List();

            _statelessSession.Transact(session =>
            {
                foreach (var log in logs)
                {
                    session.Delete(log);
                }
            });
        }
    }
}