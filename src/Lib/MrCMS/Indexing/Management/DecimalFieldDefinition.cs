using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public abstract class DecimalFieldDefinition<T1, T2> : FieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        protected DecimalFieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES)
            : base(luceneSettingsService, name, store)
        {
        }

        public override FieldDefinition<T2> GetDefinition
        {
            get { return new DecimalFieldDefinition<T2>(Name, GetValues, GetValues, Store, Boost); }
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
            Func<List<T>, Dictionary<T, IEnumerable<decimal>>> getAllValues, Field.Store store, 
            float boost = 1)
        {
            GetAllValues = getAllValues;
            FieldName = fieldName;
            GetValues = getValues;
            Store = store;
            Boost = boost;
        }

        public Func<List<T>, Dictionary<T, IEnumerable<decimal>>> GetAllValues { get; set; }
        public Func<T, IEnumerable<decimal>> GetValues { get; set; }

        public override List<IIndexableField> GetFields(T obj)
        {
            return GetFields(GetValues(obj));
        }

        public override Dictionary<T, List<IIndexableField>> GetFields(List<T> obj)
        {
            Dictionary<T, IEnumerable<decimal>> values = GetAllValues(obj);
            return values.ToDictionary(pair => pair.Key,
                pair => GetFields(pair.Value));
        }

        private List<IIndexableField> GetFields(IEnumerable<decimal> values)
        {
            return values.Select(
                value =>
                    new DoubleField(FieldName, Convert.ToDouble(value), Store) { Boost = Boost }).Cast<IIndexableField>().ToList();
        }
    }
}