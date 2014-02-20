using System.IO;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Management
{
    public class FSDirectoryIndexManager<TEntity, TDefinition> : IndexManager<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        private FSDirectory _directory;

        public FSDirectoryIndexManager(Site currentSite, TDefinition definition)
            : base(currentSite, definition)
        {
        }

        protected override Directory GetDirectory(Site site)
        {
            return _directory = _directory ?? FSDirectory.Open(new DirectoryInfo(Definition.GetLocation(site)));
        }
    }
}