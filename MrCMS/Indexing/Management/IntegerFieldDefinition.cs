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
            get { return new IntegerFieldDefinition<T2>(Name, arg => GetValues(arg), Store, Index, Boost); }
        }

        protected abstract IEnumerable<int> GetValues(T2 obj);
    }
    public class IntegerFieldDefinition<T> : FieldDefinition<T>
    {
        public Func<T, IEnumerable<int>> GetValues { get; set; }
        public IntegerFieldDefinition(string fieldName, Func<T, IEnumerable<int>> getValues, Field.Store store, Field.Index index, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = getValues;
            Store = store;
            Index = index;
            Boost = boost;
        }

        public IntegerFieldDefinition(string fieldName, Func<T, int> getValue, Field.Store store, Field.Index index, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = arg => new List<int> { getValue(arg) };
            Store = store;
            Index = index;
            Boost = boost;
        }
        

        public override List<AbstractField> GetFields(T obj)
        {
            var values = GetValues(obj).ToList();
            return
                values.Select(value => new NumericField(FieldName, Store, Index != Field.Index.NO) { Boost = Boost }.SetIntValue(value))
                      .Cast<AbstractField>()
                      .ToList();
        }
    }
}