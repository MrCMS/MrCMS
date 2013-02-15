using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Indexing.Management
{
    public interface IIndexDefinition<T> where T : class
    {
        Document Convert(T entity);
        Term GetIndex(T entity);
        T Convert(ISession session, Document document);
        IEnumerable<T> Convert(ISession session, IEnumerable<Document> documents);

        string GetLocation(CurrentSite currentSite);
        Analyzer GetAnalyser();

        string Name { get; }
    }
}