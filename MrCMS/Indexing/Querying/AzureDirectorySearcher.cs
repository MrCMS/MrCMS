using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Indexing.Querying
{
    public class AzureDirectorySearcher<TEntity, TDefinition> : Searcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        private static AzureDirectory _directory;
        private readonly IAzureFileSystem _azureFileSystem;

        public AzureDirectorySearcher(Site currentSite, ISession session, TDefinition definition,
                                      IAzureFileSystem azureFileSystem, SiteSettings siteSettings)
            : base(currentSite, session, definition, siteSettings)
        {
            _azureFileSystem = azureFileSystem;
        }

        protected override Directory GetDirectory(Site currentSite)
        {
            return
                _directory =
                _directory ??
                new AzureDirectory(_azureFileSystem.StorageAccount, "Indexes-" + IndexFolderName, new RAMDirectory());
        }
    }
}