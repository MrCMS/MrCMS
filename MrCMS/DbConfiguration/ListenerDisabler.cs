using System;
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
        private readonly IMergeEventListener[] _saveOrUpdateCopyEventListeners;
        private readonly ISaveOrUpdateEventListener[] _saveOrUpdateEventListeners;
        private readonly ISession _session;
        private readonly ISaveOrUpdateEventListener[] _updateEventListeners;

        public ListenerDisabler(ISession session)
        {
            _session = session;
            EventListeners eventListeners = _session.GetSessionImplementation().Listeners;

            _loadEventListeners = eventListeners.LoadEventListeners;
            eventListeners.LoadEventListeners = new ILoadEventListener[0];

            _saveOrUpdateEventListeners = eventListeners.SaveOrUpdateEventListeners;
            eventListeners.SaveOrUpdateEventListeners = new ISaveOrUpdateEventListener[0];

            _saveOrUpdateCopyEventListeners = eventListeners.SaveOrUpdateCopyEventListeners;
            eventListeners.SaveOrUpdateCopyEventListeners = new IMergeEventListener[0];

            _mergeEventListeners = eventListeners.MergeEventListeners;
            eventListeners.MergeEventListeners = new IMergeEventListener[0];

            _persistEventListeners = eventListeners.PersistEventListeners;
            eventListeners.PersistEventListeners = new IPersistEventListener[0];

            _persistOnFlushEventListeners = eventListeners.PersistOnFlushEventListeners;
            eventListeners.PersistOnFlushEventListeners = new IPersistEventListener[0];

            _replicateEventListeners = eventListeners.ReplicateEventListeners;
            eventListeners.ReplicateEventListeners = new IReplicateEventListener[0];

            _deleteEventListeners = eventListeners.DeleteEventListeners;
            eventListeners.DeleteEventListeners = new IDeleteEventListener[0];

            _autoFlushEventListeners = eventListeners.AutoFlushEventListeners;
            eventListeners.AutoFlushEventListeners = new IAutoFlushEventListener[0];

            _dirtyCheckEventListeners = eventListeners.DirtyCheckEventListeners;
            eventListeners.DirtyCheckEventListeners = new IDirtyCheckEventListener[0];

            _flushEventListeners = eventListeners.FlushEventListeners;
            eventListeners.FlushEventListeners = new IFlushEventListener[0];

            _evictEventListeners = eventListeners.EvictEventListeners;
            eventListeners.EvictEventListeners = new IEvictEventListener[0];

            _lockEventListeners = eventListeners.LockEventListeners;
            eventListeners.LockEventListeners = new ILockEventListener[0];

            _refreshEventListeners = eventListeners.RefreshEventListeners;
            eventListeners.RefreshEventListeners = new IRefreshEventListener[0];

            _flushEntityEventListeners = eventListeners.FlushEntityEventListeners;
            eventListeners.FlushEntityEventListeners = new IFlushEntityEventListener[0];

            _initializeCollectionEventListeners = eventListeners.InitializeCollectionEventListeners;
            eventListeners.InitializeCollectionEventListeners = new IInitializeCollectionEventListener[0];

            _postLoadEventListeners = eventListeners.PostLoadEventListeners;
            eventListeners.PostLoadEventListeners = new IPostLoadEventListener[0];

            _preLoadEventListeners = eventListeners.PreLoadEventListeners;
            eventListeners.PreLoadEventListeners = new IPreLoadEventListener[0];

            _preDeleteEventListeners = eventListeners.PreDeleteEventListeners;
            eventListeners.PreDeleteEventListeners = new IPreDeleteEventListener[0];

            _preUpdateEventListeners = eventListeners.PreUpdateEventListeners;
            eventListeners.PreUpdateEventListeners = new IPreUpdateEventListener[0];

            _preInsertEventListeners = eventListeners.PreInsertEventListeners;
            eventListeners.PreInsertEventListeners = new IPreInsertEventListener[0];

            _postDeleteEventListeners = eventListeners.PostDeleteEventListeners;
            eventListeners.PostDeleteEventListeners = new IPostDeleteEventListener[0];

            _postUpdateEventListeners = eventListeners.PostUpdateEventListeners;
            eventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[0];

            _postInsertEventListeners = eventListeners.PostInsertEventListeners;
            eventListeners.PostInsertEventListeners = new IPostInsertEventListener[0];

            _postCommitDeleteEventListeners = eventListeners.PostCommitDeleteEventListeners;
            eventListeners.PostCommitDeleteEventListeners = new IPostDeleteEventListener[0];

            _postCommitUpdateEventListeners = eventListeners.PostCommitUpdateEventListeners;
            eventListeners.PostCommitUpdateEventListeners = new IPostUpdateEventListener[0];

            _postCommitInsertEventListeners = eventListeners.PostCommitInsertEventListeners;
            eventListeners.PostCommitInsertEventListeners = new IPostInsertEventListener[0];

            _saveEventListeners = eventListeners.SaveEventListeners;
            eventListeners.SaveEventListeners = new ISaveOrUpdateEventListener[0];

            _updateEventListeners = eventListeners.UpdateEventListeners;
            eventListeners.UpdateEventListeners = new ISaveOrUpdateEventListener[0];

            _preCollectionRecreateEventListeners = eventListeners.PreCollectionRecreateEventListeners;
            eventListeners.PreCollectionRecreateEventListeners = new IPreCollectionRecreateEventListener[0];

            _postCollectionRecreateEventListeners = eventListeners.PostCollectionRecreateEventListeners;
            eventListeners.PostCollectionRecreateEventListeners = new IPostCollectionRecreateEventListener[0];

            _preCollectionRemoveEventListeners = eventListeners.PreCollectionRemoveEventListeners;
            eventListeners.PreCollectionRemoveEventListeners = new IPreCollectionRemoveEventListener[0];

            _postCollectionRemoveEventListeners = eventListeners.PostCollectionRemoveEventListeners;
            eventListeners.PostCollectionRemoveEventListeners = new IPostCollectionRemoveEventListener[0];

            _preCollectionUpdateEventListeners = eventListeners.PreCollectionUpdateEventListeners;
            eventListeners.PreCollectionUpdateEventListeners = new IPreCollectionUpdateEventListener[0];

            _postCollectionUpdateEventListeners = eventListeners.PostCollectionUpdateEventListeners;
            eventListeners.PostCollectionUpdateEventListeners = new IPostCollectionUpdateEventListener[0];
        }

        public void Dispose()
        {
            EventListeners eventListeners = _session.GetSessionImplementation().Listeners;
            eventListeners.LoadEventListeners = _loadEventListeners;
            eventListeners.SaveOrUpdateEventListeners = _saveOrUpdateEventListeners;
            eventListeners.SaveOrUpdateCopyEventListeners = _saveOrUpdateCopyEventListeners;
            eventListeners.MergeEventListeners = _mergeEventListeners;
            eventListeners.PersistEventListeners = _persistEventListeners;
            eventListeners.PersistOnFlushEventListeners = _persistOnFlushEventListeners;
            eventListeners.ReplicateEventListeners = _replicateEventListeners;
            eventListeners.DeleteEventListeners = _deleteEventListeners;
            eventListeners.AutoFlushEventListeners = _autoFlushEventListeners;
            eventListeners.DirtyCheckEventListeners = _dirtyCheckEventListeners;
            eventListeners.FlushEventListeners = _flushEventListeners;
            eventListeners.EvictEventListeners = _evictEventListeners;
            eventListeners.LockEventListeners = _lockEventListeners;
            eventListeners.RefreshEventListeners = _refreshEventListeners;
            eventListeners.FlushEntityEventListeners = _flushEntityEventListeners;
            eventListeners.InitializeCollectionEventListeners = _initializeCollectionEventListeners;
            eventListeners.PostLoadEventListeners = _postLoadEventListeners;
            eventListeners.PreLoadEventListeners = _preLoadEventListeners;
            eventListeners.PreDeleteEventListeners = _preDeleteEventListeners;
            eventListeners.PreUpdateEventListeners = _preUpdateEventListeners;
            eventListeners.PreInsertEventListeners = _preInsertEventListeners;
            eventListeners.PostDeleteEventListeners = _postDeleteEventListeners;
            eventListeners.PostUpdateEventListeners = _postUpdateEventListeners;
            eventListeners.PostInsertEventListeners = _postInsertEventListeners;
            eventListeners.PostCommitDeleteEventListeners = _postCommitDeleteEventListeners;
            eventListeners.PostCommitUpdateEventListeners = _postCommitUpdateEventListeners;
            eventListeners.PostCommitInsertEventListeners = _postCommitInsertEventListeners;
            eventListeners.SaveEventListeners = _saveEventListeners;
            eventListeners.UpdateEventListeners = _updateEventListeners;
            eventListeners.PreCollectionRecreateEventListeners = _preCollectionRecreateEventListeners;
            eventListeners.PostCollectionRecreateEventListeners = _postCollectionRecreateEventListeners;
            eventListeners.PreCollectionRemoveEventListeners = _preCollectionRemoveEventListeners;
            eventListeners.PostCollectionRemoveEventListeners = _postCollectionRemoveEventListeners;
            eventListeners.PreCollectionUpdateEventListeners = _preCollectionUpdateEventListeners;
            eventListeners.PostCollectionUpdateEventListeners = _postCollectionUpdateEventListeners;
        }
    }
}