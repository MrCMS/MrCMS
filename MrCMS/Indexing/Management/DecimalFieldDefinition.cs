using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public abstract class DecimalFieldDefinition<T1, T2> : FieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        protected DecimalFieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES, Field.Index index = Field.Index.ANALYZED)
            : base(luceneSettingsService, name, store, index)
        {
        }

        public override FieldDefinition<T2> GetDefinition
        {
            get { return new DecimalFieldDefinition<T2>(Name, GetValues, GetValues, Store, Index, Boost); }
        }

        protected abstract IEnumerable<decimal> GetValues(T2 obj);

        protected virtual Dictionary<T2, IEnumerable<decimal>> GetValues(List<T2> objs)
        {
            return objs.ToDictionary(arg => arg, GetValues);
        }
    }

    public class DecimalFieldDefinition<T> : FieldDefinition<T>
    {
        public DecimalFieldDefinition(string fieldName, Func<T, IEnumerable<decimal>> getValues,
            Func<List<T>, Dictionary<T, IEnumerable<decimal>>> getAllValues, Field.Store store, Field.Index index,
            float boost = 1)
        {
            GetAllValues = getAllValues;
            FieldName = fieldName;
            GetValues = getValues;
            Store = store;
            Index = index;
            Boost = boost;
        }

        public Func<List<T>, Dictionary<T, IEnumerable<decimal>>> GetAllValues { get; set; }
        public Func<T, IEnumerable<decimal>> GetValues { get; set; }

        public override List<AbstractField> GetFields(T obj)
        {
            return GetFields(GetValues(obj));
        }

        public override Dictionary<T, List<AbstractField>> GetFields(List<T> obj)
        {
            Dictionary<T, IEnumerable<decimal>> values = GetAllValues(obj);
            return values.ToDictionary(pair => pair.Key,
                pair => GetFields(pair.Value));
        }

        private List<AbstractField> GetFields(IEnumerable<decimal> values)
        {
            return values.Select(
                value =>
                    new NumericField(FieldName, Store, Index != Field.Index.NO) {Boost = Boost}.SetDoubleValue(
                        Convert.ToDouble(value))).Cast<AbstractField>().ToList();
        }
    }
}