using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.DbConfiguration
{
    public static class CoreEventPublisher
    {
        public static void Publish(this EventInfo eventInfo, ISession session, Type eventType,
            Func<EventInfo, ISession, Type, object> getArgs)
        {
            Type type = eventInfo.GetType().GenericTypeArguments[0];

            List<Type> types = GetEntityTypes(type).Reverse().ToList();

            types.ForEach(
                t => EventContext.Instance.Publish(eventType.MakeGenericType(t), getArgs(eventInfo, session, t)));
        }

        public static void Publish(this UpdatedEventInfo updatedEventInfo, ISession session, Type eventType,
            Func<UpdatedEventInfo, ISession, Type, object> getArgs)
        {
            Type type = updatedEventInfo.GetType().GenericTypeArguments[0];

            List<Type> types = GetEntityTypes(type).Reverse().ToList();

            types.ForEach(
                t => EventContext.Instance.Publish(eventType.MakeGenericType(t), getArgs(updatedEventInfo, session, t)));
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