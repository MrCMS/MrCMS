using System.IO;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using NHibernate;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Querying
{
    public class FSDirectorySearcher<TEntity, TDefinition> : Searcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IIndexDefinition<TEntity>, new()
    {
        public FSDirectorySearcher(CurrentSite currentSite, ISession session) : base(currentSite, session)
        {
        }

        protected override Directory GetDirectory(CurrentSite currentSite)
        {
            return FSDirectory.Open(new DirectoryInfo(Definition.GetLocation(currentSite)));
        }
    }
}