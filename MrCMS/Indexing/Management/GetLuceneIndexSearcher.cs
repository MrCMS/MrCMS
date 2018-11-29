using Lucene.Net.Search;
using MrCMS.Entities.Multisite;
using System.Collections.Concurrent;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneIndexSearcher : IGetLuceneIndexSearcher
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, IndexSearcher>> IndexSearcherCache =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, IndexSearcher>>();

        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly Site _site;

        public GetLuceneIndexSearcher(IGetLuceneDirectory getLuceneDirectory, Site site)
        {
            _getLuceneDirectory = getLuceneDirectory;
            _site = site;
        }

        private int SiteId
        {
            get { return _site.Id; }
        }

        public IndexSearcher Get(IndexDefinition definition)
        {
            return Get(definition.IndexFolderName);
        }

        public IndexSearcher Get(string folderName)
        {

            return IndexSearcherCache.GetOrAdd(SiteId, new ConcurrentDictionary<string, IndexSearcher>()).GetOrAdd(folderName, GetInternal(folderName));
        }

        public void Reset(IndexDefinition definition)
        {
            Reset(definition.IndexFolderName);
        }

        public void Reset(string folderName)
        {
            var removed = IndexSearcherCache.GetOrAdd(SiteId, new ConcurrentDictionary<string, IndexSearcher>())
                  .TryRemove(folderName, out var searcher);

            if (removed)
            {
                searcher.Dispose();
            }
        }

        public void ClearCache()
        {
            foreach (var key in IndexSearcherCache.Keys)
            {
                if (!IndexSearcherCache.TryRemove(key, out var dictionary))
                    continue;

                foreach (var dictionaryKey in dictionary.Keys)
                {
                    if(dictionary.TryRemove(dictionaryKey,out var searcher))
                        searcher.Dispose();
                }
            }
        }

        private IndexSearcher GetInternal(string folderName)
        {
            return new IndexSearcher(_getLuceneDirectory.GetRamDirectory(_site, folderName));
        }
    }
}