using System;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Tasks
{
    public abstract class AdHocTask
    {
        public abstract int Priority { get; }

        public async Task<TaskExecutionResult> Execute()
        {
            await OnExecute();
            return TaskExecutionResult.Successful(this);
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

        protected abstract Task OnExecute();
    }
}