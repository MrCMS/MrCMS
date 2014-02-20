using System;
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
        private AzureDirectory _directory;
        private readonly IAzureFileSystem _azureFileSystem;

        public AzureDirectorySearcher(Site currentSite, ISession session, TDefinition definition,
                                      IAzureFileSystem azureFileSystem, SiteSettings siteSettings)
            : base(currentSite, definition, siteSettings)
        {
            _azureFileSystem = azureFileSystem;
        }

        protected override Directory GetDirectory(Site site)
        {
            var catalog = AzureDirectoryHelper.GetAzureCatalogName(site, IndexFolderName);
            try
            {
                return
                    _directory =
                        _directory ??
                        new AzureDirectory(_azureFileSystem.StorageAccount, catalog, new RAMDirectory());
            }
            catch (Exception ex)
            {
                throw new Exception("Tried to create catalog " + catalog, ex);
            }
        }
    }
}