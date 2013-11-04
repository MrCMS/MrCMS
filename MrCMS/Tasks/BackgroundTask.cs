using System;
using MrCMS.Entities.Multisite;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public abstract class BackgroundTask
    {
        protected BackgroundTask(Site site)
        {
            Site = site;
        }

        protected ISession Session;

        public Site Site { get; set; }

        protected virtual void Initialize(ISession session)
        {
            Session = session;
        }

        protected virtual void OnError(Exception e)
        {
        }

        public bool? Run(ISession openSession)
        {
            Initialize(openSession);
            try
            {
                Session.Transact(session => Execute());
                TaskExecutor.StartExecuting();
                return true;
            }
            catch (Exception e)
            {
                OnError(e);
                return false;
            }
            finally
            {
                TaskExecutor.Discard();
            }
        }

        public abstract void Execute();
    }
}