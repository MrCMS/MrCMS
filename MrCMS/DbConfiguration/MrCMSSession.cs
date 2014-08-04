using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MrCMS.Entities;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Stat;
using NHibernate.Transaction;
using NHibernate.Type;

namespace MrCMS.DbConfiguration
{
    public class MrCMSSession : ISession
    {
        private readonly ISession _session;

        public MrCMSSession(ISession session)
        {
            _session = session;
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        public void Flush()
        {
            _session.Flush();
        }

        public IDbConnection Disconnect()
        {
            return _session.Disconnect();
        }

        public void Reconnect()
        {
            _session.Reconnect();
        }

        public void Reconnect(IDbConnection connection)
        {
            _session.Reconnect(connection);
        }

        public IDbConnection Close()
        {
            return _session.Close();
        }

        public void CancelQuery()
        {
            _session.CancelQuery();
        }

        public bool IsDirty()
        {
            return _session.IsDirty();
        }

        public bool IsReadOnly(object entityOrProxy)
        {
            return _session.IsReadOnly(entityOrProxy);
        }

        public void SetReadOnly(object entityOrProxy, bool readOnly)
        {
            _session.SetReadOnly(entityOrProxy, readOnly);
        }

        public object GetIdentifier(object obj)
        {
            return _session.GetIdentifier(obj);
        }

        public bool Contains(object obj)
        {
            return _session.Contains(obj);
        }

        public void Evict(object obj)
        {
            _session.Evict(obj);
        }

        public object Load(Type theType, object id, LockMode lockMode)
        {
            return _session.Load(theType, id, lockMode);
        }

        public object Load(string entityName, object id, LockMode lockMode)
        {
            return _session.Load(entityName, id, lockMode);
        }

        public object Load(Type theType, object id)
        {
            return _session.Load(theType, id);
        }

        public T Load<T>(object id, LockMode lockMode)
        {
            return _session.Load<T>(id, lockMode);
        }

        public T Load<T>(object id)
        {
            return _session.Load<T>(id);
        }

        public object Load(string entityName, object id)
        {
            return _session.Load(entityName, id);
        }

        public void Load(object obj, object id)
        {
            _session.Load(obj, id);
        }

        public void Replicate(object obj, ReplicationMode replicationMode)
        {
            _session.Replicate(obj, replicationMode);
        }

        public void Replicate(string entityName, object obj, ReplicationMode replicationMode)
        {
            _session.Replicate(entityName, obj, replicationMode);
        }
        private readonly HashSet<EventInfo> _added = new HashSet<EventInfo>();
        private readonly HashSet<UpdatedEventInfo> _updated = new HashSet<UpdatedEventInfo>();
        private readonly HashSet<EventInfo> _deleted = new HashSet<EventInfo>();
        public object Save(object obj)
        {
            AddAddEvent(obj);
            return _session.Save(obj);
        }

        private void AddAddEvent(object obj)
        {
            if (Added.All(info => info.Object != obj))
                Added.Add(new EventInfo { Object = obj });
        }

        public void Save(object obj, object id)
        {
            _session.Save(obj, id);
        }

        public object Save(string entityName, object obj)
        {
            return _session.Save(entityName, obj);
        }

        public void SaveOrUpdate(object obj)
        {
            var entity = obj as SystemEntity;
            if (entity != null && entity.Id == 0)
                AddAddEvent(entity);
            else
                AddUpdateEvent(obj);
            _session.SaveOrUpdate(obj);
        }

        public void SaveOrUpdate(string entityName, object obj)
        {
            _session.SaveOrUpdate(entityName, obj);
        }

        public void Update(object obj)
        {
            AddUpdateEvent(obj);
            _session.Update(obj);
        }

        private void AddUpdateEvent(object obj)
        {
            if (Updated.All(info => info.Object != obj))
                Updated.Add(new UpdatedEventInfo { Object = obj, OriginalVersion = GetOriginalVersion(obj) });
        }

        private object GetOriginalVersion(object o)
        {
            if (o == null)
                return null;
            var sessionImplementation = GetSessionImplementation();
            var persistenceContext = sessionImplementation.PersistenceContext;
            var entry = persistenceContext.GetEntry(o);
            if (entry == null)
                return null;
            var loadedState = entry.LoadedState;
            if (loadedState == null)
                return null;
            var type = o.GetType();
            var instance = Activator.CreateInstance(type);
            var entityPersister = entry.Persister;
            for (int index = 0; index < entityPersister.PropertyNames.Length; index++)
            {
                var propertyName = entityPersister.PropertyNames[index];
                var value = loadedState[index];
                var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var propertyInfo = propertyInfos.FirstOrDefault(info => info.Name == propertyName);
                try
                {
                    if (propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(instance, value);
                    }
                }
                catch
                {
                    throw new Exception(propertyName + " not found");
                }
            }
            return instance;
        }

        public void Update(object obj, object id)
        {
            _session.Update(obj, id);
        }

        public void Update(string entityName, object obj)
        {
            _session.Update(entityName, obj);
        }

        public object Merge(object obj)
        {
            return _session.Merge(obj);
        }

        public object Merge(string entityName, object obj)
        {
            return _session.Merge(entityName, obj);
        }

        public T Merge<T>(T entity) where T : class
        {
            return _session.Merge(entity);
        }

        public T Merge<T>(string entityName, T entity) where T : class
        {
            return _session.Merge(entityName, entity);
        }

        public void Persist(object obj)
        {
            _session.Persist(obj);
        }

        public void Persist(string entityName, object obj)
        {
            _session.Persist(entityName, obj);
        }

        public object SaveOrUpdateCopy(object obj)
        {
            return _session.SaveOrUpdateCopy(obj);
        }

        public object SaveOrUpdateCopy(object obj, object id)
        {
            return _session.SaveOrUpdateCopy(obj, id);
        }

        public void Delete(object obj)
        {
            if (Deleted.All(info => info.Object != obj))
                Deleted.Add(new EventInfo { Object = obj });
            //EventContext.Instance.Publish<IOnDeleting, OnDeletingArgs>(new OnDeletingArgs
            //{
            //    Item = obj as SystemEntity,
            //    Session = this
            //});
            _session.Delete(obj);
        }

        public void Delete(string entityName, object obj)
        {
            _session.Delete(entityName, obj);
        }

        public int Delete(string query)
        {
            return _session.Delete(query);
        }

        public int Delete(string query, object value, IType type)
        {
            return _session.Delete(query, value, type);
        }

        public int Delete(string query, object[] values, IType[] types)
        {
            return _session.Delete(query, values, types);
        }

        public void Lock(object obj, LockMode lockMode)
        {
            _session.Lock(obj, lockMode);
        }

        public void Lock(string entityName, object obj, LockMode lockMode)
        {
            _session.Lock(entityName, obj, lockMode);
        }

        public void Refresh(object obj)
        {
            _session.Refresh(obj);
        }

        public void Refresh(object obj, LockMode lockMode)
        {
            _session.Refresh(obj, lockMode);
        }

        public LockMode GetCurrentLockMode(object obj)
        {
            return _session.GetCurrentLockMode(obj);
        }

        public ITransaction BeginTransaction()
        {
            return new MrCMSTransaction(_session.BeginTransaction(), this);
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return _session.BeginTransaction(isolationLevel);
        }

        public ICriteria CreateCriteria<T>() where T : class
        {
            return _session.CreateCriteria<T>();
        }

        public ICriteria CreateCriteria<T>(string alias) where T : class
        {
            return _session.CreateCriteria<T>(alias);
        }

        public ICriteria CreateCriteria(Type persistentClass)
        {
            return _session.CreateCriteria(persistentClass);
        }

        public ICriteria CreateCriteria(Type persistentClass, string alias)
        {
            return _session.CreateCriteria(persistentClass, alias);
        }

        public ICriteria CreateCriteria(string entityName)
        {
            return _session.CreateCriteria(entityName);
        }

        public ICriteria CreateCriteria(string entityName, string alias)
        {
            return _session.CreateCriteria(entityName, alias);
        }

        public IQueryOver<T, T> QueryOver<T>() where T : class
        {
            return _session.QueryOver<T>();
        }

        public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class
        {
            return _session.QueryOver(alias);
        }

        public IQueryOver<T, T> QueryOver<T>(string entityName) where T : class
        {
            return _session.QueryOver<T>(entityName);
        }

        public IQueryOver<T, T> QueryOver<T>(string entityName, Expression<Func<T>> alias) where T : class
        {
            return _session.QueryOver(entityName, alias);
        }

        public IQuery CreateQuery(string queryString)
        {
            return _session.CreateQuery(queryString);
        }

        public IQuery CreateFilter(object collection, string queryString)
        {
            return _session.CreateFilter(collection, queryString);
        }

        public IQuery GetNamedQuery(string queryName)
        {
            return _session.GetNamedQuery(queryName);
        }

        public ISQLQuery CreateSQLQuery(string queryString)
        {
            return _session.CreateSQLQuery(queryString);
        }

        public void Clear()
        {
            _session.Clear();
        }

        public object Get(Type clazz, object id)
        {
            return _session.Get(clazz, id);
        }

        public object Get(Type clazz, object id, LockMode lockMode)
        {
            return _session.Get(clazz, id, lockMode);
        }

        public object Get(string entityName, object id)
        {
            return _session.Get(entityName, id);
        }

        public T Get<T>(object id)
        {
            return _session.Get<T>(id);
        }

        public T Get<T>(object id, LockMode lockMode)
        {
            return _session.Get<T>(id, lockMode);
        }

        public string GetEntityName(object obj)
        {
            return _session.GetEntityName(obj);
        }

        public IFilter EnableFilter(string filterName)
        {
            return _session.EnableFilter(filterName);
        }

        public IFilter GetEnabledFilter(string filterName)
        {
            return _session.GetEnabledFilter(filterName);
        }

        public void DisableFilter(string filterName)
        {
            _session.DisableFilter(filterName);
        }

        public IMultiQuery CreateMultiQuery()
        {
            return _session.CreateMultiQuery();
        }

        public ISession SetBatchSize(int batchSize)
        {
            return _session.SetBatchSize(batchSize);
        }

        public ISessionImplementor GetSessionImplementation()
        {
            return _session.GetSessionImplementation();
        }

        public IMultiCriteria CreateMultiCriteria()
        {
            return _session.CreateMultiCriteria();
        }

        public ISession GetSession(EntityMode entityMode)
        {
            return _session.GetSession(entityMode);
        }

        public EntityMode ActiveEntityMode
        {
            get { return _session.ActiveEntityMode; }
        }

        public FlushMode FlushMode
        {
            get { return _session.FlushMode; }
            set { _session.FlushMode = value; }
        }

        public CacheMode CacheMode
        {
            get { return _session.CacheMode; }
            set { _session.CacheMode = value; }
        }

        public ISessionFactory SessionFactory
        {
            get { return _session.SessionFactory; }
        }

        public IDbConnection Connection
        {
            get { return _session.Connection; }
        }

        public bool IsOpen
        {
            get { return _session.IsOpen; }
        }

        public bool IsConnected
        {
            get { return _session.IsConnected; }
        }

        public bool DefaultReadOnly
        {
            get { return _session.DefaultReadOnly; }
            set { _session.DefaultReadOnly = value; }
        }

        public ITransaction Transaction
        {
            get { return _session.Transaction; }
        }

        public ISessionStatistics Statistics
        {
            get { return _session.Statistics; }
        }

        public HashSet<EventInfo> Added
        {
            get { return _added; }
        }

        public HashSet<UpdatedEventInfo> Updated
        {
            get { return _updated; }
        }

        public HashSet<EventInfo> Deleted
        {
            get { return _deleted; }
        }
    }

    public class EventInfo
    {
        public object Object { get; set; }
        public bool PreTransactionHandled { get; set; }
        public bool PostTransactionHandled { get; set; }
    }
    public class UpdatedEventInfo : EventInfo
    {
        public object OriginalVersion { get; set; }
    }

    public class MrCMSTransaction : ITransaction
    {
        private readonly ITransaction _transaction;
        private readonly MrCMSSession _session;

        public MrCMSTransaction(ITransaction transaction, MrCMSSession session)
        {
            _transaction = transaction;
            _session = session;
        }

        public void Dispose()
        {
            _transaction.Dispose();
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
            HandlePreTransaction(_session);
            _transaction.Commit();
            HandlePostTransaction(_session);
        }

        private static void HandlePostTransaction(MrCMSSession session)
        {
            var eventInfos = session.Added.ToHashSet();
            eventInfos.ForEach(obj =>
            {
                if (obj.PostTransactionHandled)
                    return;
                obj.PostTransactionHandled = true;
                EventContext.Instance.Publish<IOnAdded, OnAddedArgs>(new OnAddedArgs
                {
                    Item = obj.Object as SystemEntity,
                    Session = session
                });
                session.Added.Remove(obj);
            });
            var updatedEventInfos = session.Updated.ToHashSet();
            updatedEventInfos.ForEach(obj =>
            {
                if (obj.PostTransactionHandled)
                    return;
                obj.PostTransactionHandled = true;
                EventContext.Instance.Publish<IOnUpdated, OnUpdatedArgs>(new OnUpdatedArgs
                {
                    Original = obj.OriginalVersion as SystemEntity,
                    Item = obj.Object as SystemEntity,
                    Session = session
                });
                session.Updated.Remove(obj);
            });
            var hashSet = session.Deleted.ToHashSet();
            hashSet.ForEach(obj =>
            {
                if (obj.PostTransactionHandled)
                    return;
                obj.PostTransactionHandled = true;
                EventContext.Instance.Publish<IOnDeleted, OnDeletedArgs>(new OnDeletedArgs
                {
                    Item = obj.Object as SystemEntity,
                    Session = session
                });
                session.Deleted.Remove(obj);
            });
        }

        private static void HandlePreTransaction(MrCMSSession session)
        {
            var eventInfos = session.Added.ToHashSet();
            eventInfos.ForEach(obj =>
            {
                if (obj.PreTransactionHandled)
                    return;
                obj.PreTransactionHandled = true;
                EventContext.Instance.Publish<IOnAdding, OnAddingArgs>(new OnAddingArgs
                {
                    Item = obj.Object as SystemEntity,
                    Session = session
                });
            });
            var updatedEventInfos = session.Updated.ToHashSet();
            updatedEventInfos.ForEach(obj =>
            {
                if (obj.PreTransactionHandled)
                    return;
                obj.PreTransactionHandled = true;
                EventContext.Instance.Publish<IOnUpdating, OnUpdatingArgs>(new OnUpdatingArgs
                {
                    Original = obj.OriginalVersion as SystemEntity,
                    Item = obj.Object as SystemEntity,
                    Session = session
                });
            });
            var hashSet = session.Deleted.ToHashSet();
            hashSet.ForEach(obj =>
            {
                if (obj.PreTransactionHandled)
                    return;
                obj.PreTransactionHandled = true;
                EventContext.Instance.Publish<IOnDeleting, OnDeletingArgs>(new OnDeletingArgs
                {
                    Item = obj.Object as SystemEntity,
                    Session = session
                });
            });
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Enlist(IDbCommand command)
        {
            _transaction.Enlist(command);
        }

        public void RegisterSynchronization(ISynchronization synchronization)
        {
            _transaction.RegisterSynchronization(synchronization);
        }

        public bool IsActive
        {
            get { return _transaction.IsActive; }
        }

        public bool WasRolledBack
        {
            get { return _transaction.WasRolledBack; }
        }

        public bool WasCommitted
        {
            get { return _transaction.WasCommitted; }
        }
    }
}