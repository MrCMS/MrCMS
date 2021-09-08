using System.Threading.Tasks;

namespace MrCMS.Events
{
    public interface IEvent
    {
    }
    public interface IEvent<in T> : IEvent
    {
        Task Execute(T args);
    }
}