using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Services;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.DbConfiguration
{
    public static class CoreEventPublisher
    {
        public static async Task Publish(this EventInfo eventInfo, ISession session, Type eventType,
            Func<EventInfo, ISession, Type, object> getArgs)
        {
            Type type = eventInfo.GetType().GenericTypeArguments[0];

            List<Type> types = GetEntityTypes(type).Reverse().ToList();

            foreach (var t in types)
            {
                var eventContext = session.GetService<IEventContext>();
                var makeGenericType = eventType.MakeGenericType(t);
                var args = getArgs(eventInfo, session, t);
                await eventContext.Publish(makeGenericType, args);
            }
        }

        public static async Task Publish(this UpdatedEventInfo updatedEventInfo, ISession session, Type eventType,
            Func<UpdatedEventInfo, ISession, Type, object> getArgs)
        {
            Type type = updatedEventInfo.GetType().GenericTypeArguments[0];

            List<Type> types = GetEntityTypes(type).Reverse().ToList();

            foreach (var t in types)
            {
                await session.GetService<IEventContext>()
                    .Publish(eventType.MakeGenericType(t), getArgs(updatedEventInfo, session, t));
            }
        }


        private static IEnumerable<Type> GetEntityTypes(Type type)
        {
            Type thisType = type;
            while (thisType != null && thisType != typeof(SystemEntity))
            {
                yield return thisType;
                thisType = thisType.BaseType;
            }

            yield return typeof(SystemEntity);
        }
    }
}