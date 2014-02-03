using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Services;

namespace MrCMS.Indexing.Management
{
    public class AzureDirectoryIndexManager<TEntity, TDefinition> : IndexManager<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IIndexDefinition<TEntity>
    {
        private readonly IAzureFileSystem _azureFileSystem;
        private static AzureDirectory _directory;

        public AzureDirectoryIndexManager(Site currentSite, TDefinition definition, IAzureFileSystem azureFileSystem)
            : base(currentSite, definition)
        {
            _azureFileSystem = azureFileSystem;
        }

        protected override Directory GetDirectory()
        {
            return _directory = _directory ?? new AzureDirectory(_azureFileSystem.StorageAccount, "Indexes-" + IndexFolderName, new RAMDirectory());
        }
    }
}