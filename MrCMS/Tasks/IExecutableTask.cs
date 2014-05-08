using System;
using MrCMS.Entities.Multisite;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public interface IExecutableTask
    {
        int Priority { get; }
        bool Schedulable { get; }
        Site Site { get; set; }
        IHaveExecutionStatus Entity { get; set; }
        TaskExecutionResult Execute();
        string GetData();
        void SetData(string data);

        void OnFailure(Exception exception);
        void OnSuccess();
        void OnFinalFailure(Exception exception);
        void OnStarting();
    }

    public abstract class BaseExecutableTask : IExecutableTask
    {
        public abstract int Priority { get; }
        public abstract bool Schedulable { get; }

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

    public abstract class AdHocTask : BaseExecutableTask
    {
        public override sealed bool Schedulable
        {
            get { return false; }
        }
    }

    public abstract class SchedulableTask : BaseExecutableTask
    {
        public override sealed bool Schedulable
        {
            get { return true; }
        }

        public override sealed string GetData()
        {
            return string.Empty;
        }

        public override sealed void SetData(string data)
        {
        }
    }
}