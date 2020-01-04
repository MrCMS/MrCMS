using System;
using MrCMS.Entities;

namespace MrCMS.Tasks
{
    public interface IHaveExecutionStatus : IHaveId
    {
        void OnStarting(AdHocTask executableTask);
        void OnSuccess(AdHocTask executableTask);
        void OnFailure(AdHocTask executableTask, Exception exception);
    }
}