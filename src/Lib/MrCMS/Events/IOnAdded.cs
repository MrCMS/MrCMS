using MrCMS.Entities;

namespace MrCMS.Events
{
    /// <summary>
    /// Interface to define events that are called after a new item has been persisted to the db
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOnAdded<T> : ICoreEvent, IEvent<OnAddedArgs<T>> where T : SystemEntity
    {
    }
}