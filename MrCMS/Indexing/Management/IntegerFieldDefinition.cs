using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;

namespace MrCMS.Indexing.Management
{
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