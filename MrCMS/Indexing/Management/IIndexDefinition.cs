using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using NHibernate;
using System.Linq;

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
        IEnumerable<FieldDefinition<T>> Definitions { get; }

        string IndexName { get; }
    }

    public class FieldDefinition
    {
        public static string[] GetFields(params FieldDefinition[] definitions)
        {
            return definitions.Select(definition => definition.FieldName).ToArray();
        }
        public string FieldName { get; set; }
        public Field.Store Store { get; set; }
        public Field.Index Index { get; set; }
    }
    public class FieldDefinition<T> :FieldDefinition
    {
        public FieldDefinition(string fieldName, Func<T, string> getValue, Field.Store store, Field.Index index)
        {
            FieldName = fieldName;
            GetValue = getValue;
            Store = store;
            Index = index;
        }
        
        public Func<T, string> GetValue { get; set; }
    }
}