using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class EventContext : IEventContext
    {
        public static IEventContext Instance { get { return MrCMSApplication.Get<IEventContext>(); } }

        private readonly IEnumerable<IEvent> _events;

        public EventContext(IEnumerable<IEvent> events)
        {
            _events = events;
        }

        public void Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>
        {
            _events.OfType<TEvent>().ForEach(obj => obj.Execute(args));
        }

        public void Publish(Type eventType, object args)
        {
            _events.Where(@eventType.IsInstanceOfType).ForEach(@event =>
            {
                var methodInfo = @event.GetType().GetMethod("Execute", new[] {args.GetType()});
                methodInfo.Invoke(@event, new[] {args});
            });
        }
    }
}