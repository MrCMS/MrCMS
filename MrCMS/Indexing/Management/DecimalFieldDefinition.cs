using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;

namespace MrCMS.Indexing.Management
{
    public class DecimalFieldDefinition<T> : FieldDefinition<T>
    {
        public Func<T, IEnumerable<decimal>> GetValues { get; set; }
        public DecimalFieldDefinition(string fieldName, Func<T, IEnumerable<decimal>> getValues, Field.Store store, Field.Index index, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = getValues;
            Store = store;
            Index = index;
            Boost = boost;
        }

        public DecimalFieldDefinition(string fieldName, Func<T, decimal> getValue, Field.Store store, Field.Index index, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = arg => new List<decimal> { getValue(arg) };
            Store = store;
            Index = index;
            Boost = boost;
        }
        public override List<AbstractField> GetFields(T obj)
        {
            var values = GetValues(obj).ToList();
            return
                values.Select(
                    value =>
                    new NumericField(FieldName, Store, Index != Field.Index.NO) { Boost = Boost }.SetDoubleValue(
                        Convert.ToDouble(value))).Cast<AbstractField>().ToList();
        }
    }
}