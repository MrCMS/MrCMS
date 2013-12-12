using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Indexing.Querying
{
    public class AzureDirectorySearcher<TEntity, TDefinition> : Searcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IIndexDefinition<TEntity>, new()
    {
        private readonly IAzureFileSystem _azureFileSystem;
        private static AzureDirectory _directory;

        public AzureDirectorySearcher(Site currentSite, ISession session, IAzureFileSystem azureFileSystem)
            : base(currentSite, session)
        {
            _azureFileSystem = azureFileSystem;
        }

        protected override Directory GetDirectory(Site currentSite)
        {
            return _directory = _directory ?? new AzureDirectory(_azureFileSystem.StorageAccount, "Indexes-" + IndexFolderName, new RAMDirectory());
        }
    }
}