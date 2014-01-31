namespace MrCMS.Tasks
{
    public interface ITaskStatusUpdater
    {
        void BeginExecution(IExecutableTask executableTask);
        void SuccessfulCompletion(IExecutableTask executableTask);
        void FailedExecution(IExecutableTask executableTask);
    }
}