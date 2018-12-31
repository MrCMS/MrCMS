namespace MrCMS.Tasks
{
    public enum TaskExecutionStatus
    {
        Pending,
        AwaitingExecution,
        Executing,
        Completed,
        Failed
    }
}