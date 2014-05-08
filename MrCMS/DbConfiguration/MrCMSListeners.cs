using MrCMS.DbConfiguration.Configuration;

namespace MrCMS.DbConfiguration
{
    public class MrCMSListeners
    {
        private static readonly SaveOrUpdateListener _saveOrUpdateListener = new SaveOrUpdateListener();
        private static readonly UpdateIndicesListener _updateIndexesListener = new UpdateIndicesListener();
        private static readonly PostCommitEventListener _postCommitEventListener = new PostCommitEventListener();
        private static readonly UrlHistoryListener _urlHistoryListener = new UrlHistoryListener();
        private static SoftDeleteListener _softDeleteListener;

        public static SaveOrUpdateListener SaveOrUpdateListener
        {
            get { return _saveOrUpdateListener; }
        }

        public static UpdateIndicesListener UpdateIndexesListener
        {
            get { return _updateIndexesListener; }
        }

        public static PostCommitEventListener PostCommitEventListener
        {
            get { return _postCommitEventListener; }
        }

        public static UrlHistoryListener UrlHistoryListener
        {
            get { return _urlHistoryListener; }
        }
    }
}