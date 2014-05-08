using System;

namespace MrCMS.Tasks
{
    public interface IHaveExecutionStatus
    {
        void OnStarting(IExecutableTask executableTask);
        void OnSuccess(IExecutableTask executableTask);
        void OnFailure(IExecutableTask executableTask, Exception exception);
    }
}