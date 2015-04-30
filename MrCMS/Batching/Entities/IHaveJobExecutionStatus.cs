namespace MrCMS.Batching.Entities
{
    public interface IHaveJobExecutionStatus
    {
        JobExecutionStatus Status { get; set; }
    }
}