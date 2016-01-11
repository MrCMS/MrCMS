using System;
using MrCMS.Entities.Multisite;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public abstract class AdHocTask
    {
        public abstract int Priority { get; }

        public TaskExecutionResult Execute()
        {
            try
            {
                OnExecute();
                return TaskExecutionResult.Successful(this);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return TaskExecutionResult.Failure(this, ex);
            }
        }

        public abstract string GetData();
        public abstract void SetData(string data);
        public Site Site { get; set; }
        public IHaveExecutionStatus Entity { get; set; }

        public virtual void OnFailure(Exception exception)
        {
        }

        public virtual void OnSuccess()
        {
        }

        public virtual void OnFinalFailure(Exception exception)
        {
        }

        public virtual void OnStarting()
        {
        }

        protected abstract void OnExecute();
    }
}