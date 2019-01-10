using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public abstract class IntegerFieldDefinition<T1, T2> : FieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        protected IntegerFieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES)
            : base(luceneSettingsService, name, store)
        {
        }

        public override FieldDefinition<T2> GetDefinition
        {
            get { return new IntegerFieldDefinition<T2>(Name, GetValues, GetValues, Store, Boost); }
        }

        protected abstract IEnumerable<int> GetValues(T2 obj);

        protected virtual Dictionary<T2, IEnumerable<int>> GetValues(List<T2> objs)
        {
            return objs.ToDictionary(arg => arg, GetValues);
        }
    }
    public class IntegerFieldDefinition<T> : FieldDefinition<T>
    {
        public Func<T, IEnumerable<int>> GetValues { get; set; }
        public Func<List<T>, Dictionary<T, IEnumerable<int>>> GetAllValues { get; set; }
        public IntegerFieldDefinition(string fieldName, Func<T, IEnumerable<int>> getValues, Func<List<T>, Dictionary<T, IEnumerable<int>>> getAllValues, Field.Store store, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = getValues;
            GetAllValues = getAllValues;
            Store = store;
            Boost = boost;
        }


        public override List<IIndexableField> GetFields(T obj)
        {
            return GetFields(GetValues(obj));
        }

        public override Dictionary<T, List<IIndexableField>> GetFields(List<T> obj)
        {
            var values = GetAllValues(obj);
            return values.ToDictionary(pair => pair.Key,
                pair => GetFields(pair.Value));
        }


        private List<IIndexableField> GetFields(IEnumerable<int> values)
        {
            return values.Select(value => new Int32Field(FieldName,value, Store) { Boost = Boost })
                .Cast<IIndexableField>()
                .ToList();
        }
    }
}