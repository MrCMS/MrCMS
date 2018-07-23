using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public abstract class StringFieldDefinition<T1, T2> : FieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        protected StringFieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES, Field.Index index = Field.Index.ANALYZED)
            : base(luceneSettingsService, name, store)
        {
        }

        public override FieldDefinition<T2> GetDefinition
        {
            get { return new StringFieldDefinition<T2>(Name, GetValues, GetValues, Store, Boost); }
        }

        protected abstract IEnumerable<string> GetValues(T2 obj);

        protected virtual Dictionary<T2, IEnumerable<string>> GetValues(List<T2> objs)
        {
            return objs.ToDictionary(arg => arg, GetValues);
        }
    }

    public class StringFieldDefinition<T> : FieldDefinition<T>
    {
        public StringFieldDefinition(string fieldName, Func<T, IEnumerable<string>> getValues, Func<List<T>, Dictionary<T, IEnumerable<string>>> getAllValues,
            Field.Store store, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = getValues;
            GetAllValues = getAllValues;
            Store = store;
            Boost = boost;
        }

        public Func<T, IEnumerable<string>> GetValues { get; set; }
        public Func<List<T>, Dictionary<T, IEnumerable<string>>> GetAllValues { get; set; }

        public override List<IIndexableField> GetFields(T obj)
        {
            return GetFields(GetValues(obj));
        }

        public override Dictionary<T, List<IIndexableField>> GetFields(List<T> obj)
        {
            List<KeyValuePair<T, IEnumerable<string>>> values = GetAllValues(obj).ToList();
            return values.ToDictionary(pair => pair.Key,
                pair => GetFields(pair.Value));
        }

        private List<IIndexableField> GetFields(IEnumerable<string> values)
        {
            return values.Select(s => new TextField(FieldName, s ?? string.Empty, Store) { Boost = Boost })
                .Cast<IIndexableField>()
                .ToList();
        }
    }
}