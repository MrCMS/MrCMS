using System;
using System.Threading.Tasks;
using MrCMS.Services;

namespace MrCMS.Events
{
    public class NullEventContext : IEventContext
    {
        public Task Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>
        {
            return Task.FromResult(0);
        }

        public Task Publish(Type eventType, object args)
        {
            return Task.FromResult(0);
        }

        public IDisposable Disable<T>()
        {
            return Task.FromResult(0);
        }

        public IDisposable Disable(params Type[] types)
        {
            return Task.FromResult(0);
        }
    }
}