using System;
using System.Linq;
using MrCMS.Events;
using MrCMS.Services;

namespace MrCMS.Helpers
{
    public static class EventContextExtensions
    {
        public static IDisposable DisableAll(this IEventContext eventContext)
        {
            return eventContext.Disable(GetAllEvents());
        }

        public static IDisposable DisableCoreEvents(this IEventContext eventContext)
        {
            return eventContext.Disable(GetCoreEvents());
        }

        public static IDisposable DisableNonCoreEvents(this IEventContext eventContext)
        {
            return eventContext.Disable(GetAllEvents().Except(GetCoreEvents()).ToArray());
        }

        private static Type[] GetCoreEvents()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<ICoreEvent>().ToArray();
        }

        private static Type[] GetAllEvents()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<IEvent>().ToArray();
        }
    }
}