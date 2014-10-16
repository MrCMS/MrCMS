using System;
using FakeItEasy;
using MrCMS.Events;
using MrCMS.Services;

namespace MrCMS.Web.Tests
{
    public class TestableEventContext : IEventContext
    {
        private readonly IEventContext _coreEventContext;
        private readonly IEventContext _fakeEventContext;
        public bool FakeNonCoreEvents = true;

        public TestableEventContext(IEventContext coreEventContext = null)
        {
            _fakeEventContext = A.Fake<IEventContext>();
            _coreEventContext = coreEventContext ?? _fakeEventContext;
        }

        public IEventContext FakeEventContext
        {
            get { return _fakeEventContext; }
        }

        public void Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>
        {
            if (typeof(ICoreEvent).IsAssignableFrom(typeof(TEvent)) || !FakeNonCoreEvents)
            {
                _coreEventContext.Publish<TEvent, TArgs>(args);
            }
            else
            {
                _fakeEventContext.Publish<TEvent, TArgs>(args);
            }
        }

        public void Publish(Type eventType, object args)
        {
            if (typeof(ICoreEvent).IsAssignableFrom(eventType) || !FakeNonCoreEvents)
            {
                _coreEventContext.Publish(eventType, args);
            }
            else
            {
                _fakeEventContext.Publish(eventType, args);
            }
        }

        public IDisposable Disable<T>()
        {
            return _coreEventContext.Disable<T>();
        }

        public IDisposable Disable(params Type[] types)
        {
            return _coreEventContext.Disable(types);
        }
    }
}