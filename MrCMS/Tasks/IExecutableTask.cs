using System;
using MrCMS.Entities.Multisite;

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
}