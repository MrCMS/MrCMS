using System;
using MrCMS.Entities;
using MrCMS.Events;
using NHibernate;

namespace MrCMS.DbConfiguration.Helpers
{
    public static class EventInfoExtensions
    {
        public static EventInfo GetEventInfo(this SystemEntity entity)
        {
            if (entity == null)
                return null;
            return Activator.CreateInstance(typeof(EventInfo<>).MakeGenericType(entity.GetType()), entity) as EventInfo;
        }
        public static EventInfo GetTypedInfo(this EventInfo info,Type type)
        {
            if (info == null)
                return null;
            return Activator.CreateInstance(typeof(EventInfo<>).MakeGenericType(type), info) as EventInfo;
        }

        public static UpdatedEventInfo GetUpdatedEventInfo(this SystemEntity entity, SystemEntity original)
        {
            if (entity == null)
                return null;
            return
                Activator.CreateInstance(typeof(UpdatedEventInfo<>).MakeGenericType(entity.GetType()), entity, original)
                    as UpdatedEventInfo;
        }
        public static UpdatedEventInfo GetTypedInfo(this UpdatedEventInfo info, Type type)
        {
            if (info == null)
                return null;
            return Activator.CreateInstance(typeof(UpdatedEventInfo<>).MakeGenericType(type), info) as UpdatedEventInfo;
        }

        public static OnUpdatedArgs ToUpdatedArgs(this UpdatedEventInfo info, ISession session, Type type = null)
        {
            type = type ?? info.GetType().GenericTypeArguments[0];
            return
                Activator.CreateInstance(typeof(OnUpdatedArgs<>).MakeGenericType(type), info, session) as OnUpdatedArgs;
        }

        public static OnUpdatingArgs ToUpdatingArgs(this UpdatedEventInfo info, ISession session, Type type = null)
        {
            type = type ?? info.GetType().GenericTypeArguments[0];
            return
                Activator.CreateInstance(typeof(OnUpdatingArgs<>).MakeGenericType(type), info, session) as
                    OnUpdatingArgs;
        }

        public static OnAddedArgs ToAddedArgs(this EventInfo info, ISession session, Type type = null)
        {
            type = type ?? info.GetType().GenericTypeArguments[0];
            return
                Activator.CreateInstance(typeof(OnAddedArgs<>).MakeGenericType(type), info, session) as OnAddedArgs;
        }

        public static OnAddingArgs ToAddingArgs(this EventInfo info, ISession session, Type type = null)
        {
            type = type ?? info.GetType().GenericTypeArguments[0];
            return
                Activator.CreateInstance(typeof(OnAddingArgs<>).MakeGenericType(type), info, session) as OnAddingArgs;
        }

        public static OnDeletedArgs ToDeletedArgs(this EventInfo info, ISession session, Type type = null)
        {
            type = type ?? info.GetType().GenericTypeArguments[0];
            return
                Activator.CreateInstance(typeof(OnDeletedArgs<>).MakeGenericType(type), info, session) as OnDeletedArgs;
        }

        public static OnDeletingArgs ToDeletingArgs(this EventInfo info, ISession session, Type type = null)
        {
            type = type ?? info.GetType().GenericTypeArguments[0];
            return
                Activator.CreateInstance(typeof(OnDeletingArgs<>).MakeGenericType(type), info, session) as
                    OnDeletingArgs;
        }
    }
}