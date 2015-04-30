using Lucene.Net.Search;
using MrCMS.Entities.Multisite;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneIndexSearcher : IGetLuceneIndexSearcher
    {
        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly Site _site;

        public GetLuceneIndexSearcher(IGetLuceneDirectory getLuceneDirectory, Site site)
        {
            _getLuceneDirectory = getLuceneDirectory;
            _site = site;
        }

        public IndexSearcher Get(string folderName)
        {
            return new IndexSearcher(_getLuceneDirectory.Get(_site, folderName, true));
        }

        public int SiteId { get { return _site.Id; }}
    }
}