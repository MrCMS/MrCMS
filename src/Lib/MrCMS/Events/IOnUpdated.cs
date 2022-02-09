using MrCMS.Entities;

namespace MrCMS.Events
{
    /// <summary>
    /// Interface to define events that are called after an item has been updated in the db
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOnUpdated<T> : ICoreEvent, IEvent<OnUpdatedArgs<T>> where T : SystemEntity
    {
    }
}