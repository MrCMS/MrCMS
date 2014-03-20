using System.Collections.Generic;
using System.Linq;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class EventContext : IEventContext
    {
        /// <summary>
        /// Added to allow this to be used with DI. If preferred, it can still be injected as an IEventContext 
        /// </summary>
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
    }
}