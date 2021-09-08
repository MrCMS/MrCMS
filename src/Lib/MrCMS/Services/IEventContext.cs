using System;
using System.Threading.Tasks;
using MrCMS.Events;

namespace MrCMS.Services
{
    public interface IEventContext
    {
        Task Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>;
        Task Publish(Type eventType, object args);

        /// <summary>
        /// Disables the publishing of events of the specified type in the event context until the result is disposed
        /// </summary>
        /// <returns></returns>
        IDisposable Disable<T>();

        /// <summary>
        /// Disables the publishing of events of the specified type in the event context until the result is disposed
        /// </summary>
        /// <returns></returns>
        IDisposable Disable(params Type[] types);

        IDisposable DisableAll();
    }
}