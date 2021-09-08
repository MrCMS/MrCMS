using Microsoft.AspNetCore.Http;
using MrCMS.DbConfiguration.Helpers;
using MrCMS.Entities;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using NHibernate.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISession = NHibernate.ISession;

namespace MrCMS.DbConfiguration
{
    public class MrCMSTransaction : ITransaction
    {
        private readonly MrCMSSession _session;
        private readonly ITransaction _transaction;

        public MrCMSTransaction(ITransaction transaction, MrCMSSession session)
        {
            _transaction = transaction;
            _session = session;
        }

        public HttpContext HttpContext => _session.HttpContext;

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await HandlePreTransaction(_session);
            await _transaction.CommitAsync(cancellationToken);
            if (!MrCMSTransactionWrapper.IsInPostTransaction(HttpContext))
            {
                using (new MrCMSTransactionWrapper(HttpContext))
                {
                    await HandlePostTransaction(_session);
                }
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await _transaction.RollbackAsync(cancellationToken);
        }

        public void Begin()
        {
            _transaction.Begin();
        }

        public void Begin(IsolationLevel isolationLevel)
        {
            _transaction.Begin(isolationLevel);
        }

        public void Commit()
        {
            throw new NotImplementedException("Refactor to use async");
            // HandlePreTransaction(_session);
            // _transaction.Commit();
            // if (!MrCMSTransactionWrapper.IsInPostTransaction(HttpContext))
            // {
            //     using (new MrCMSTransactionWrapper(HttpContext))
            //     {
            //         await HandlePostTransaction(_session);
            //     }
            // }
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Enlist(DbCommand command)
        {
            _transaction.Enlist(command);
        }

#pragma warning disable 618
        public void RegisterSynchronization(ISynchronization synchronization)
        {
            _transaction.RegisterSynchronization(synchronization);
        }
#pragma warning restore 618

        public bool IsActive => _transaction.IsActive;

        public bool WasRolledBack => _transaction.WasRolledBack;

        public bool WasCommitted => _transaction.WasCommitted;

        private static async Task HandlePostTransaction(MrCMSSession session)
        {
            while (GetNextAddedEventInfoForPostTransaction(session) != null)
            {
                EventInfo obj = GetNextAddedEventInfoForPostTransaction(session);
                obj.PostTransactionHandled = true;
                await Publish(obj, session, typeof(IOnAdded<>),
                    (info, ses, t) => info.GetTypedInfo(t).ToAddedArgs(ses, t));
            }

            while (GetNextUpdatedEventInfoForPostTransaction(session) != null)
            {
                UpdatedEventInfo obj = GetNextUpdatedEventInfoForPostTransaction(session);
                obj.PostTransactionHandled = true;
                await Publish(obj, session, typeof(IOnUpdated<>),
                    (info, ses, t) => info.GetTypedInfo(t).ToUpdatedArgs(ses, t));
            }

            while (GetNextDeletedEventInfoForPostTransaction(session) != null)
            {
                EventInfo obj = GetNextDeletedEventInfoForPostTransaction(session);
                obj.PostTransactionHandled = true;
                await Publish(obj, session, typeof(IOnDeleted<>),
                    (info, ses, t) => info.GetTypedInfo(t).ToDeletedArgs(ses, t));
            }
        }

        private static EventInfo GetNextAddedEventInfoForPostTransaction(MrCMSSession session)
        {
            return session.Added.FirstOrDefault(x => !x.PostTransactionHandled);
        }

        private static UpdatedEventInfo GetNextUpdatedEventInfoForPostTransaction(MrCMSSession session)
        {
            return session.Updated.FirstOrDefault(x => !x.PostTransactionHandled);
        }

        private static EventInfo GetNextDeletedEventInfoForPostTransaction(MrCMSSession session)
        {
            return session.Deleted.FirstOrDefault(x => !x.PostTransactionHandled);
        }

        private static async Task Publish<T>(T onUpdatedArgs, ISession session, Type eventType,
            Func<T, ISession, Type, object> getArgs)
        {
            Type type = onUpdatedArgs.GetType().GenericTypeArguments[0];

            List<Type> types = GetEntityTypes(type).Reverse().ToList();

            foreach (var t in types)
            {
                await session.GetService<IEventContext>()
                    .Publish(eventType.MakeGenericType(t), getArgs(onUpdatedArgs, session, t));
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

        private static async Task HandlePreTransaction(MrCMSSession session)
        {
            HashSet<EventInfo> eventInfos = session.Added.ToHashSet();
            foreach (var obj in eventInfos)
            {
                if (obj.PreTransactionHandled)
                {
                    continue;
                }

                obj.PreTransactionHandled = true;
                await Publish(obj, session, typeof(IOnAdding<>),
                    (info, ses, t) => info.GetTypedInfo(t).ToAddingArgs(ses, t));
            }

            HashSet<UpdatedEventInfo> updatedEventInfos = session.Updated.ToHashSet();
            foreach (var obj in updatedEventInfos)
            {
                if (obj.PreTransactionHandled)
                {
                    continue;
                }

                obj.PreTransactionHandled = true;
                await Publish(obj, session, typeof(IOnUpdating<>),
                    (info, ses, t) => info.GetTypedInfo(t).ToUpdatingArgs(ses, t));
            }

            HashSet<EventInfo> hashSet = session.Deleted.ToHashSet();
            foreach (var obj in hashSet)
            {
                if (obj.PreTransactionHandled)
                {
                    continue;
                }

                obj.PreTransactionHandled = true;
                await Publish(obj, session, typeof(IOnDeleting<>),
                    (info, ses, t) => info.GetTypedInfo(t).ToDeletingArgs(ses, t));
            }
        }

        private class MrCMSTransactionWrapper : IDisposable
        {
            private const string InPostTransactionKey = "in-post-transaction";
            private readonly bool _completeOnDispose;
            private readonly HttpContext _httpContext;

            public MrCMSTransactionWrapper(HttpContext httpContext)
            {
                _httpContext = httpContext;
                if (!IsInPostTransaction(_httpContext))
                {
                    _completeOnDispose = true;
                    BeginPostTransaction(_httpContext);
                }
            }

            public void Dispose()
            {
                if (_completeOnDispose)
                {
                    CompletedPostTransaction(_httpContext);
                }
            }

            private static void BeginPostTransaction(HttpContext context)
            {
                if (context != null)
                {
                    context.Items[InPostTransactionKey] = true;
                }
            }

            private static void CompletedPostTransaction(HttpContext context)
            {
                if (context != null)
                {
                    context.Items.Remove(InPostTransactionKey);
                }
            }

            public static bool IsInPostTransaction(HttpContext context)
            {
                return context != null && context.Items.ContainsKey(InPostTransactionKey);
            }
        }
    }
}