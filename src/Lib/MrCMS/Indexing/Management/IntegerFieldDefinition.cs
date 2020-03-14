using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected abstract IAsyncEnumerable<int> GetValues(T2 obj);

        protected virtual Dictionary<T2, IAsyncEnumerable<int>> GetValues(List<T2> objs)
        {
            return objs.ToDictionary(arg => arg, GetValues);
        }
    }
    public class IntegerFieldDefinition<T> : FieldDefinition<T>
    {
        public Func<T, IAsyncEnumerable<int>> GetValues { get; set; }
        public Func<List<T>, Dictionary<T, IAsyncEnumerable<int>>> GetAllValues { get; set; }
        public IntegerFieldDefinition(string fieldName, Func<T, IAsyncEnumerable<int>> getValues, Func<List<T>, Dictionary<T, IAsyncEnumerable<int>>> getAllValues, Field.Store store, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = getValues;
            GetAllValues = getAllValues;
            Store = store;
            Boost = boost;
        }


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


        private async Task<List<IIndexableField>> GetFields(IAsyncEnumerable<int> values)
        {

            var list = new List<IIndexableField>();
            await foreach (var value in values)
            {
                list.Add(new Int32Field(FieldName, (value), Store) { Boost = Boost });
            }

            return list;
        }
    }
}