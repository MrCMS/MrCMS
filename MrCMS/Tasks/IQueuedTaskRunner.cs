namespace MrCMS.Tasks
{
    public interface IQueuedTaskRunner
    {
        BatchExecutionResult ExecutePendingTasks();
        BatchExecutionResult ExecuteLuceneTasks();
    }
}