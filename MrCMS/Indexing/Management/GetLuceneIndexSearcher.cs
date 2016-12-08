using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using MrCMS.Entities.Multisite;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneIndexSearcher : IGetLuceneIndexSearcher
    {
        private static readonly Dictionary<int, Dictionary<string, IndexSearcher>> IndexSearcherCache =
            new Dictionary<int, Dictionary<string, IndexSearcher>>();

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
            if (!IndexSearcherCache.ContainsKey(SiteId))
            {
                IndexSearcherCache[SiteId] = new Dictionary<string, IndexSearcher>();
            }
            var dictionary = IndexSearcherCache[SiteId];
            if (!dictionary.ContainsKey(folderName))
            {
                dictionary[folderName] = GetInternal(folderName);
            }
            return dictionary[folderName];
        }

        public void Reset(IndexDefinition definition)
        {
            Reset(definition.IndexFolderName);
        }

        public void Reset(string folderName)
        {
            if (IndexSearcherCache.ContainsKey(SiteId))
            {
                IndexSearcherCache[SiteId].Remove(folderName);
            }
        }

        public void ClearCache()
        {
            foreach (var indexSearcher in IndexSearcherCache.SelectMany(x => x.Value.Values))
                indexSearcher.Dispose();

            IndexSearcherCache.Clear();
        }

        public IndexSearcher GetInternal(string folderName)
        {
            return new IndexSearcher(_getLuceneDirectory.Get(_site, folderName, true));
        }
    }
}