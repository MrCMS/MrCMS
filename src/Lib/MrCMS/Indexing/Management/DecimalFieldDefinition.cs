using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected abstract IAsyncEnumerable<decimal> GetValues(T2 obj);

        protected virtual Dictionary<T2, IAsyncEnumerable<decimal>> GetValues(List<T2> objs)
        {
            return objs.ToDictionary(arg => arg, GetValues);
        }
    }

    public class DecimalFieldDefinition<T> : FieldDefinition<T>
    {
        public DecimalFieldDefinition(string fieldName, Func<T, IAsyncEnumerable<decimal>> getValues,
            Func<List<T>, Dictionary<T, IAsyncEnumerable<decimal>>> getAllValues, Field.Store store, 
            float boost = 1)
        {
            GetAllValues = getAllValues;
            FieldName = fieldName;
            GetValues = getValues;
            Store = store;
            Boost = boost;
        }

        public Func<T, IAsyncEnumerable<decimal>> GetValues { get; set; }
        public Func<List<T>, Dictionary<T, IAsyncEnumerable<decimal>>> GetAllValues { get; set; }

        public override async Task<List<IIndexableField>> GetFields(T obj)
        {
            return await GetFields(GetValues(obj));
        }

        public override async Task<Dictionary<T, List<IIndexableField>>> GetFields(List<T> obj)
        {
            var pairs = GetAllValues(obj).ToList();

            var dictionary = new Dictionary<T, List<IIndexableField>>();
            foreach (var pair in pairs)
            {
                dictionary.Add(pair.Key, await GetFields(pair.Value));
            }
            return dictionary;
        }

        private async Task<List<IIndexableField>> GetFields(IAsyncEnumerable<decimal> values)
        {
            var list = new List<IIndexableField>();
            await foreach (var value in values)
            {
                list.Add(new DoubleField(FieldName, Convert.ToDouble(value), Store) {Boost = Boost});
            }

            return list;
        }
    }
}