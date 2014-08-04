using System;
using MrCMS.Events;

namespace MrCMS.Services
{
    public interface IEventContext
    {
        void Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>;
        void Publish(Type eventType, object args);
    }
}