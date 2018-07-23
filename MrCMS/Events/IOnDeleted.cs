using MrCMS.Entities;

namespace MrCMS.Events
{
    /// <summary>
    /// Interface to define events that are called after an item has been deleted
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOnDeleted<T> : ICoreEvent, IEvent<OnDeletedArgs<T>> where T : SystemEntity
    {
    }
}