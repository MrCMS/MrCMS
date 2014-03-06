namespace MrCMS.Tasks
{
    public interface IHaveExecutionStatus
    {
        void OnStarting();
        void OnSuccess();
        void OnFailure();
    }
}