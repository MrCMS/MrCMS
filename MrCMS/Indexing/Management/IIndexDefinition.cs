using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace MrCMS.Indexing.Management
{
    public interface IIndexDefinition<in T> where T : class
    {
        Document Convert(T entity);
        Term GetIndex(T entity);
        string GetLocation();
        Analyzer GetAnalyser();
    }
}