using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Events;
using MrCMS.Helpers;
using NHibernate.Util;

namespace MrCMS.Services
{
    public class EventContext : IEventContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HashSet<Type> _disabledEvents = new HashSet<Type>();

        public EventContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //public static IEventContext Instance
        //{
        //    get
        //    {
        //        try
        //        {
        //            //return MrCMSApplication.Get<IEventContext>();
        //            return null;
        //        }
        //        catch
        //        {
        //            return new InstallationEventContext();
        //        }
        //    }
        //}

        public HashSet<Type> DisabledEvents
        {
            get { return _disabledEvents; }
        }

        public void Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>
        {
            Publish(typeof (TEvent), args);
        }

        public void Publish(Type eventType, object args)
        {
            //using (MiniProfiler.Current.Step("Publishing " + eventType.FullName))
            {
                foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom(eventType).Where(type => !IsDisabled(type)))
                {
                    //using (MiniProfiler.Current.Step("Invoking " + @event.GetType().FullName))
                    {
                        MethodInfo methodInfo = type.GetMethod("Execute", new[] {args.GetType()});
                        var instance = _serviceProvider.GetRequiredService(type);
                        methodInfo.Invoke(instance, new[] {args});
                    }
                }
            }
        }

        public IDisposable Disable<T>()
        {
            return new EventPublishingDisabler(this, typeof (T));
        }

        public IDisposable Disable(params Type[] types)
        {
            return new EventPublishingDisabler(this, types);
        }

        private bool IsDisabled(Type type)
        {
            return DisabledEvents.Any(type.IsImplementationOf);
        }

        public class EventPublishingDisabler : IDisposable
        {
            private readonly EventContext _eventContext;
            private readonly HashSet<Type> _toEnableOnDispose = new HashSet<Type>();

            public EventPublishingDisabler(EventContext eventContext, params Type[] types)
            {
                _eventContext = eventContext;
                foreach (Type type in types.Where(type => _eventContext.DisabledEvents.Add(type)))
                {
                    _toEnableOnDispose.Add(type);
                }
            }

            public void Dispose()
            {
                foreach (Type type in _toEnableOnDispose)
                {
                    _eventContext.DisabledEvents.Remove(type);
                }
            }
        }

        //private class InstallationEventContext : IEventContext
        //{
        //    public void Publish<TEvent, TArgs>(TArgs args) where TEvent : IEvent<TArgs>
        //    {
        //    }

        //    public void Publish(Type eventType, object args)
        //    {
        //    }

        //    public IDisposable Disable<T>()
        //    {
        //        return new DummyDisabler();
        //    }

        //    public IDisposable Disable(params Type[] types)
        //    {
        //        return new DummyDisabler();
        //    }

        //    private class DummyDisabler : IDisposable
        //    {
        //        public void Dispose()
        //        {
        //        }
        //    }
        //}
    }
}