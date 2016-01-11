namespace MrCMS.Tasks
{
    public interface IQueuedTaskRunner
    {
        void TriggerPendingTasks();

        BatchExecutionResult ExecutePendingTasks();
        BatchExecutionResult ExecuteLuceneTasks();
    }
}