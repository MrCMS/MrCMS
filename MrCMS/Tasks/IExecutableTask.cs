using System;
using MrCMS.Entities.Multisite;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public interface IExecutableTask
    {
        int Priority { get; }
        bool Schedulable { get; }
        TaskExecutionResult Execute();
        string GetData();
        void SetData(string data);

        Site Site { get; set; }
        IHaveExecutionStatus Entity { get; set; }
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
                return new TaskExecutionResult {Success = true};
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return new TaskExecutionResult{Exception =ex,Success = false};
            }
        }

        protected abstract void OnExecute();
        public abstract string GetData();
        public abstract void SetData(string data);
        public Site Site { get; set; }
        public IHaveExecutionStatus Entity { get; set; }
    }

    public abstract partial class AdHocTask : BaseExecutableTask
    {
        public sealed override bool Schedulable { get { return false; } }
    }

    public abstract partial class SchedulableTask : BaseExecutableTask
    {
        public sealed override bool Schedulable { get { return true; } }
        public sealed override string GetData()
        {
            return string.Empty;
        }

        public sealed override void SetData(string data)
        {
        }
    }
}