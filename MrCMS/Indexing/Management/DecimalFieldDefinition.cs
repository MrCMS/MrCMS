using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;

namespace MrCMS.Indexing.Management
{
    public class DecimalFieldDefinition<T> : FieldDefinition<T>
    {
        public Func<T, IEnumerable<decimal>> GetValues { get; set; }
        public DecimalFieldDefinition(string fieldName, Func<T, IEnumerable<decimal>> getValues, Field.Store store, Field.Index index)
        {
            FieldName = fieldName;
            GetValues = getValues;
            Store = store;
            Index = index;
        }

        public DecimalFieldDefinition(string fieldName, Func<T, decimal> getValue, Field.Store store, Field.Index index)
        {
            FieldName = fieldName;
            GetValues = arg => new List<decimal> { getValue(arg) };
            Store = store;
            Index = index;
        }
        public override IEnumerable<AbstractField> GetFields(T obj)
        {
            var values = GetValues(obj);
            return values.Select(value => new NumericField(FieldName).SetDoubleValue(Convert.ToDouble((decimal) value)));
        }
    }
}