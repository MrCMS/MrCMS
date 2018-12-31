namespace MrCMS.Events
{
    public interface IEvent
    {
    }
    public interface IEvent<in T> : IEvent
    {
        void Execute(T args);
    }
}