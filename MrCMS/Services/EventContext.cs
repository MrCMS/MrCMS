using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Website;
using StackExchange.Profiling;

namespace MrCMS.Services
{
    public class EventContext : IEventContext
    {
        private readonly HashSet<IEvent> _events;
        private readonly HashSet<Type> _disabledEvents = new HashSet<Type>();

        public EventContext(IEnumerable<IEvent> events)
        {
            _events = events.ToHashSet();
        }

        public static IEventContext Instance
        {
            get { return MrCMSApplication.Get<IEventContext>(); }
        }

        public HashSet<Type> DisabledEvents
        {
            get { return _disabledEvents; }
        }

        public void Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>
        {
            Publish(typeof(TEvent), args);
        }

        public void Publish(Type eventType, object args)
        {
            using (MiniProfiler.Current.Step("Publishing " + eventType.FullName))
            {
                _events.Where(@event => @eventType.IsInstanceOfType(@event) && !IsDisabled(@event)).ForEach(@event =>
                {
                    using (MiniProfiler.Current.Step("Invoking " + @event.GetType().FullName))
                    {
                        MethodInfo methodInfo = @event.GetType().GetMethod("Execute", new[] {args.GetType()});
                        methodInfo.Invoke(@event, new[] {args});
                    }
                });
            }
        }

        private bool IsDisabled(IEvent @event)
        {
            return DisabledEvents.Any(type => @event.GetType().IsImplementationOf(type));
        }

        public IDisposable Disable<T>()
        {
            return new EventPublishingDisabler(this, typeof(T));
        }

        public IDisposable Disable(params Type[] types)
        {
            return new EventPublishingDisabler(this, types);
        }

        public class EventPublishingDisabler : IDisposable
        {
            private readonly EventContext _eventContext;
            private readonly HashSet<Type> _toEnableOnDispose = new HashSet<Type>();

            public EventPublishingDisabler(EventContext eventContext, params Type[] types)
            {
                _eventContext = eventContext;
                foreach (var type in types.Where(type => _eventContext.DisabledEvents.Add(type)))
                {
                    _toEnableOnDispose.Add(type);
                }
            }

            public void Dispose()
            {
                foreach (var type in _toEnableOnDispose)
                {
                    _eventContext.DisabledEvents.Remove(type);
                }
            }
        }
    }

}