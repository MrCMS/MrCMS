using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class DeleteOldQueuedTasks : SchedulableTask
    {
        private readonly ISessionFactory _sessionFactory;

        public DeleteOldQueuedTasks(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public override int Priority { get { return 10; } }

        protected override void OnExecute()
        {
            var statelessSession = _sessionFactory.OpenStatelessSession();
            var logs =
                statelessSession.QueryOver<QueuedTask>().Where(data => data.CompletedAt <= CurrentRequestData.Now.AddDays(-1)).List();

            using (var transaction = statelessSession.BeginTransaction())
            {
                foreach (var log in logs)
                {
                    statelessSession.Delete(log);
                }
                transaction.Commit();
            }
        }
    }
}