using System;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public abstract class BackgroundTask
    {
        protected ISession Session;

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
                using (ITransaction transation = Session.BeginTransaction())
                {
                    Execute();
                    transation.Commit();
                }
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