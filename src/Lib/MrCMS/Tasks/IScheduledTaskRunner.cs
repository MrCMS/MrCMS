using System;

namespace MrCMS.Tasks
{
    public interface IScheduledTaskRunner
    {
        void ExecuteTask(string type);
    }
}