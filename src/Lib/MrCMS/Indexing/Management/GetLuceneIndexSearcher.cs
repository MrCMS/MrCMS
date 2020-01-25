using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities.Multisite;
using MrCMS.Website;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneIndexSearcher : IGetLuceneIndexSearcher
    {
        private static readonly Dictionary<int, Dictionary<string, IndexSearcher>> IndexSearcherCache =
            new Dictionary<int, Dictionary<string, IndexSearcher>>();

        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly IGetSiteId _getSiteId;

        public GetLuceneIndexSearcher(IGetLuceneDirectory getLuceneDirectory, IGetSiteId getSiteId)
        {
            _getLuceneDirectory = getLuceneDirectory;
            _getSiteId = getSiteId;
        }

        private int SiteId
        {
            get { return _getSiteId.GetId(); }
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

        public IndexSearcher GetInternal(string folderName)
        {
            return new IndexSearcher(DirectoryReader.Open(_getLuceneDirectory.Get(SiteId, folderName)));
        }
    }
}