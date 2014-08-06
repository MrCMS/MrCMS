using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public abstract class IntegerFieldDefinition<T1, T2> : FieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        protected IntegerFieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES, Field.Index index = Field.Index.ANALYZED)
            : base(luceneSettingsService, name, store, index)
        {
        }

        public override FieldDefinition<T2> GetDefinition
        {
            get { return new IntegerFieldDefinition<T2>(Name, GetValues, GetValues, Store, Index, Boost); }
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
        public IntegerFieldDefinition(string fieldName, Func<T, IEnumerable<int>> getValues, Func<List<T>, Dictionary<T, IEnumerable<int>>> getAllValues, Field.Store store, Field.Index index, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = getValues;
            GetAllValues = getAllValues;
            Store = store;
            Index = index;
            Boost = boost;
        }


        public override List<AbstractField> GetFields(T obj)
        {
            return GetFields(GetValues(obj));
        }

        public override Dictionary<T, List<AbstractField>> GetFields(List<T> obj)
        {
            var values = GetAllValues(obj);
            return values.ToDictionary(pair => pair.Key,
                pair => GetFields(pair.Value));
        }


        private List<AbstractField> GetFields(IEnumerable<int> values)
        {
            return values.Select(value => new NumericField(FieldName, Store, Index != Field.Index.NO) { Boost = Boost }.SetIntValue(value))
                .Cast<AbstractField>()
                .ToList();
        }
    }
}