using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class EventContext : IEventContext
    {
        private readonly HashSet<IEvent> _events;
        internal bool PublishEnabled = true;

        public EventContext(IEnumerable<IEvent> events)
        {
            _events = events.ToHashSet();
        }

        public static IEventContext Instance
        {
            get { return MrCMSApplication.Get<IEventContext>(); }
        }

        public void Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>
        {
            if (!PublishEnabled) 
                return;

            _events.OfType<TEvent>().ForEach(obj => obj.Execute(args));
        }

        public void Publish(Type eventType, object args)
        {
            if (!PublishEnabled) 
                return;

            _events.Where(@eventType.IsInstanceOfType).ForEach(@event =>
            {
                MethodInfo methodInfo = @event.GetType().GetMethod("Execute", new[] {args.GetType()});
                methodInfo.Invoke(@event, new[] {args});
            });
        }

        public IDisposable Disable()
        {
            return new EventPublishingDisabler(this);
        }
    }

    public class EventPublishingDisabler : IDisposable
    {
        private readonly bool _enableOnDispose;
        private readonly EventContext _eventContext;

        public EventPublishingDisabler(EventContext eventContext)
        {
            _eventContext = eventContext;
            if (!_eventContext.PublishEnabled)
                return;
            _eventContext.PublishEnabled = false;
            _enableOnDispose = true;
        }

        public void Dispose()
        {
            if (_enableOnDispose)
            {
                _eventContext.PublishEnabled = true;
            }
        }
    }
}