using System;
using System.Linq;
using NHibernate;
using NHibernate.Event;

namespace MrCMS.DbConfiguration
{
    public class ListenerDisabler : IDisposable
    {
        private readonly IAutoFlushEventListener[] _autoFlushEventListeners;
        private readonly IDeleteEventListener[] _deleteEventListeners;
        private readonly IDirtyCheckEventListener[] _dirtyCheckEventListeners;
        private readonly IEvictEventListener[] _evictEventListeners;
        private readonly IFlushEntityEventListener[] _flushEntityEventListeners;
        private readonly IFlushEventListener[] _flushEventListeners;
        private readonly IInitializeCollectionEventListener[] _initializeCollectionEventListeners;
        private readonly ILoadEventListener[] _loadEventListeners;
        private readonly ILockEventListener[] _lockEventListeners;
        private readonly IMergeEventListener[] _mergeEventListeners;
        private readonly IPersistEventListener[] _persistEventListeners;
        private readonly IPersistEventListener[] _persistOnFlushEventListeners;
        private readonly IPostCollectionRecreateEventListener[] _postCollectionRecreateEventListeners;
        private readonly IPostCollectionRemoveEventListener[] _postCollectionRemoveEventListeners;
        private readonly IPostCollectionUpdateEventListener[] _postCollectionUpdateEventListeners;
        private readonly IPostDeleteEventListener[] _postCommitDeleteEventListeners;
        private readonly IPostInsertEventListener[] _postCommitInsertEventListeners;
        private readonly IPostUpdateEventListener[] _postCommitUpdateEventListeners;
        private readonly IPostDeleteEventListener[] _postDeleteEventListeners;
        private readonly IPostInsertEventListener[] _postInsertEventListeners;
        private readonly IPostLoadEventListener[] _postLoadEventListeners;
        private readonly IPostUpdateEventListener[] _postUpdateEventListeners;
        private readonly IPreCollectionRecreateEventListener[] _preCollectionRecreateEventListeners;
        private readonly IPreCollectionRemoveEventListener[] _preCollectionRemoveEventListeners;
        private readonly IPreCollectionUpdateEventListener[] _preCollectionUpdateEventListeners;
        private readonly IPreDeleteEventListener[] _preDeleteEventListeners;
        private readonly IPreInsertEventListener[] _preInsertEventListeners;
        private readonly IPreLoadEventListener[] _preLoadEventListeners;
        private readonly IPreUpdateEventListener[] _preUpdateEventListeners;
        private readonly IRefreshEventListener[] _refreshEventListeners;
        private readonly IReplicateEventListener[] _replicateEventListeners;
        private readonly ISaveOrUpdateEventListener[] _saveEventListeners;
        private readonly ISaveOrUpdateEventListener[] _saveOrUpdateEventListeners;
        private readonly ISession _session;
        private readonly ListenerType _type;
        private readonly ISaveOrUpdateEventListener[] _updateEventListeners;

        public ListenerDisabler(ISession session, ListenerType type, object listener)
        {
            _session = session;
            _type = type;
            EventListeners eventListeners = _session.GetSessionImplementation().Listeners;

            switch (type)
            {
                case ListenerType.Autoflush:
                    _autoFlushEventListeners = eventListeners.AutoFlushEventListeners;
                    eventListeners.AutoFlushEventListeners =
                        _autoFlushEventListeners.Where(eventListener => eventListeners != listener).ToArray();
                    break;
                case ListenerType.Merge:
                    _mergeEventListeners = eventListeners.MergeEventListeners;
                    eventListeners.MergeEventListeners =
                        _mergeEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Create:
                    _persistEventListeners = eventListeners.PersistEventListeners;
                    eventListeners.PersistEventListeners =
                        _persistEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.CreateOnFlush:
                    _persistOnFlushEventListeners = eventListeners.PersistOnFlushEventListeners;
                    eventListeners.PersistOnFlushEventListeners =
                        _persistOnFlushEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Delete:
                    _deleteEventListeners = eventListeners.DeleteEventListeners;
                    eventListeners.DeleteEventListeners =
                        _deleteEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.DirtyCheck:
                    _dirtyCheckEventListeners = eventListeners.DirtyCheckEventListeners;
                    eventListeners.DirtyCheckEventListeners =
                        _dirtyCheckEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Evict:
                    _evictEventListeners = eventListeners.EvictEventListeners;
                    eventListeners.EvictEventListeners =
                        _evictEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Flush:
                    _flushEventListeners = eventListeners.FlushEventListeners;
                    eventListeners.FlushEventListeners =
                        _flushEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.FlushEntity:
                    _flushEntityEventListeners = eventListeners.FlushEntityEventListeners;
                    eventListeners.FlushEntityEventListeners =
                        _flushEntityEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Load:
                    _loadEventListeners = eventListeners.LoadEventListeners;
                    eventListeners.LoadEventListeners =
                        _loadEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.LoadCollection:
                    _initializeCollectionEventListeners = eventListeners.InitializeCollectionEventListeners;
                    eventListeners.InitializeCollectionEventListeners =
                        _initializeCollectionEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Lock:
                    _lockEventListeners = eventListeners.LockEventListeners;
                    eventListeners.LockEventListeners =
                        _lockEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Refresh:
                    _refreshEventListeners = eventListeners.RefreshEventListeners;
                    eventListeners.RefreshEventListeners =
                        _refreshEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Replicate:
                    _replicateEventListeners = eventListeners.ReplicateEventListeners;
                    eventListeners.ReplicateEventListeners =
                        _replicateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.SaveUpdate:
                    _saveOrUpdateEventListeners = eventListeners.SaveOrUpdateEventListeners;
                    eventListeners.SaveOrUpdateEventListeners =
                        _saveOrUpdateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Save:
                    _saveEventListeners = eventListeners.SaveEventListeners;
                    eventListeners.SaveEventListeners =
                        _saveEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PreUpdate:
                    _preUpdateEventListeners = eventListeners.PreUpdateEventListeners;
                    eventListeners.PreUpdateEventListeners =
                        _preUpdateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.Update:
                    _updateEventListeners = eventListeners.UpdateEventListeners;
                    eventListeners.UpdateEventListeners =
                        _updateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PreLoad:
                    _preLoadEventListeners = eventListeners.PreLoadEventListeners;
                    eventListeners.PreLoadEventListeners =
                        _preLoadEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PreDelete:
                    _preDeleteEventListeners = eventListeners.PreDeleteEventListeners;
                    eventListeners.PreDeleteEventListeners =
                        _preDeleteEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PreInsert:
                    _preInsertEventListeners = eventListeners.PreInsertEventListeners;
                    eventListeners.PreInsertEventListeners =
                        _preInsertEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostLoad:
                    _postLoadEventListeners = eventListeners.PostLoadEventListeners;
                    eventListeners.PostLoadEventListeners =
                        _postLoadEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostInsert:
                    _postInsertEventListeners = eventListeners.PostInsertEventListeners;
                    eventListeners.PostInsertEventListeners =
                        _postInsertEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostUpdate:
                    _postUpdateEventListeners = eventListeners.PostUpdateEventListeners;
                    eventListeners.PostUpdateEventListeners =
                        _postUpdateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostDelete:
                    _postDeleteEventListeners = eventListeners.PostDeleteEventListeners;
                    eventListeners.PostDeleteEventListeners =
                        _postDeleteEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostCommitUpdate:
                    _postCommitUpdateEventListeners = eventListeners.PostCommitUpdateEventListeners;
                    eventListeners.PostCommitUpdateEventListeners =
                        _postCommitUpdateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostCommitInsert:
                    _postCommitInsertEventListeners = eventListeners.PostCommitInsertEventListeners;
                    eventListeners.PostCommitInsertEventListeners =
                        _postCommitInsertEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostCommitDelete:
                    _postCommitDeleteEventListeners = eventListeners.PostCommitDeleteEventListeners;
                    eventListeners.PostCommitDeleteEventListeners =
                        _postCommitDeleteEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PreCollectionRecreate:
                    _preCollectionRecreateEventListeners = eventListeners.PreCollectionRecreateEventListeners;
                    eventListeners.PreCollectionRecreateEventListeners =
                        _preCollectionRecreateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PreCollectionRemove:
                    _preCollectionRemoveEventListeners = eventListeners.PreCollectionRemoveEventListeners;
                    eventListeners.PreCollectionRemoveEventListeners =
                        _preCollectionRemoveEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PreCollectionUpdate:
                    _preCollectionUpdateEventListeners = eventListeners.PreCollectionUpdateEventListeners;
                    eventListeners.PreCollectionUpdateEventListeners =
                        _preCollectionUpdateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostCollectionRecreate:
                    _postCollectionRecreateEventListeners = eventListeners.PostCollectionRecreateEventListeners;
                    eventListeners.PostCollectionRecreateEventListeners =
                        _postCollectionRecreateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostCollectionRemove:
                    _postCollectionRemoveEventListeners = eventListeners.PostCollectionRemoveEventListeners;
                    eventListeners.PostCollectionRemoveEventListeners =
                        _postCollectionRemoveEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
                case ListenerType.PostCollectionUpdate:
                    _postCollectionUpdateEventListeners = eventListeners.PostCollectionUpdateEventListeners;
                    eventListeners.PostCollectionUpdateEventListeners =
                        _postCollectionUpdateEventListeners.Where(eventListener => eventListener != listener).ToArray();
                    break;
            }
        }

        public void Dispose()
        {
            EventListeners eventListeners = _session.GetSessionImplementation().Listeners;

            switch (_type)
            {
                case ListenerType.Autoflush:
                    eventListeners.AutoFlushEventListeners = _autoFlushEventListeners;
                    break;
                case ListenerType.Merge:
                    eventListeners.MergeEventListeners = _mergeEventListeners;
                    break;
                case ListenerType.Create:
                    eventListeners.PersistEventListeners = _persistEventListeners;
                    break;
                case ListenerType.CreateOnFlush:
                    eventListeners.PersistOnFlushEventListeners = _persistOnFlushEventListeners;
                    break;
                case ListenerType.Delete:
                    eventListeners.DeleteEventListeners = _deleteEventListeners;
                    break;
                case ListenerType.DirtyCheck:
                    eventListeners.DirtyCheckEventListeners = _dirtyCheckEventListeners;
                    break;
                case ListenerType.Evict:
                    eventListeners.EvictEventListeners = _evictEventListeners;
                    break;
                case ListenerType.Flush:
                    eventListeners.FlushEventListeners = _flushEventListeners;
                    break;
                case ListenerType.FlushEntity:
                    eventListeners.FlushEntityEventListeners = _flushEntityEventListeners;
                    break;
                case ListenerType.Load:
                    eventListeners.LoadEventListeners = _loadEventListeners;
                    break;
                case ListenerType.LoadCollection:
                    eventListeners.InitializeCollectionEventListeners = _initializeCollectionEventListeners;
                    break;
                case ListenerType.Lock:
                    eventListeners.LockEventListeners = _lockEventListeners;
                    break;
                case ListenerType.Refresh:
                    eventListeners.RefreshEventListeners = _refreshEventListeners;
                    break;
                case ListenerType.Replicate:
                    eventListeners.ReplicateEventListeners = _replicateEventListeners;
                    break;
                case ListenerType.SaveUpdate:
                    eventListeners.SaveOrUpdateEventListeners = _saveOrUpdateEventListeners;
                    break;
                case ListenerType.Save:
                    eventListeners.SaveEventListeners = _saveEventListeners;
                    break;
                case ListenerType.PreUpdate:
                    eventListeners.PreUpdateEventListeners = _preUpdateEventListeners;
                    break;
                case ListenerType.Update:
                    eventListeners.UpdateEventListeners = _updateEventListeners;
                    break;
                case ListenerType.PreLoad:
                    eventListeners.PreLoadEventListeners = _preLoadEventListeners;
                    break;
                case ListenerType.PreDelete:
                    eventListeners.PreDeleteEventListeners = _preDeleteEventListeners;
                    break;
                case ListenerType.PreInsert:
                    eventListeners.PreInsertEventListeners = _preInsertEventListeners;
                    break;
                case ListenerType.PostLoad:
                    eventListeners.PostLoadEventListeners = _postLoadEventListeners;
                    break;
                case ListenerType.PostInsert:
                    eventListeners.PostInsertEventListeners = _postInsertEventListeners;
                    break;
                case ListenerType.PostUpdate:
                    eventListeners.PostUpdateEventListeners = _postUpdateEventListeners;
                    break;
                case ListenerType.PostDelete:
                    eventListeners.PostDeleteEventListeners = _postDeleteEventListeners;
                    break;
                case ListenerType.PostCommitUpdate:
                    eventListeners.PostCommitUpdateEventListeners = _postCommitUpdateEventListeners;
                    break;
                case ListenerType.PostCommitInsert:
                    eventListeners.PostCommitInsertEventListeners = _postCommitInsertEventListeners;
                    break;
                case ListenerType.PostCommitDelete:
                    eventListeners.PostCommitDeleteEventListeners = _postCommitDeleteEventListeners;
                    break;
                case ListenerType.PreCollectionRecreate:
                    eventListeners.PreCollectionRecreateEventListeners = _preCollectionRecreateEventListeners;
                    break;
                case ListenerType.PreCollectionRemove:
                    eventListeners.PreCollectionRemoveEventListeners = _preCollectionRemoveEventListeners;
                    break;
                case ListenerType.PreCollectionUpdate:
                    eventListeners.PreCollectionUpdateEventListeners = _preCollectionUpdateEventListeners;
                    break;
                case ListenerType.PostCollectionRecreate:
                    eventListeners.PostCollectionRecreateEventListeners = _postCollectionRecreateEventListeners;
                    break;
                case ListenerType.PostCollectionRemove:
                    eventListeners.PostCollectionRemoveEventListeners = _postCollectionRemoveEventListeners;
                    break;
                case ListenerType.PostCollectionUpdate:
                    eventListeners.PostCollectionUpdateEventListeners = _postCollectionUpdateEventListeners;
                    break;
            }
        }
    }
}