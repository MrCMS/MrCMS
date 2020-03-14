using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public abstract class StringFieldDefinition<T1, T2> : FieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        protected StringFieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES)
            : base(luceneSettingsService, name, store)
        {
        }

        public override FieldDefinition<T2> GetDefinition => new StringFieldDefinition<T2>(Name, obj => GetValues(obj), GetValues, Store, Boost);

        protected abstract IAsyncEnumerable<string> GetValues(T2 obj);

        protected virtual Dictionary<T2, IAsyncEnumerable<string>> GetValues(List<T2> objs)
        {
            return objs.ToDictionary(arg => arg, obj => GetValues(obj));
        }
    }

    public class StringFieldDefinition<T> : FieldDefinition<T>
    {
        public StringFieldDefinition(string fieldName, Func<T, IAsyncEnumerable<string>> getValues, Func<List<T>, Dictionary<T, IAsyncEnumerable<string>>> getAllValues,
            Field.Store store, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = getValues;
            GetAllValues = getAllValues;
            Store = store;
            Boost = boost;
        }

        public Func<T, IAsyncEnumerable<string>> GetValues { get; set; }
        public Func<List<T>, Dictionary<T, IAsyncEnumerable<string>>> GetAllValues { get; set; }

        public override async Task<List<IIndexableField>> GetFields(T obj)
        {
            return await GetFields(GetValues(obj));
        }

        public override async Task<Dictionary<T, List<IIndexableField>>> GetFields(List<T> obj)
        {
            List<KeyValuePair<T, IAsyncEnumerable<string>>> pairs = GetAllValues(obj).ToList();

            var dictionary = new Dictionary<T, List<IIndexableField>>();
            foreach (var pair in pairs)
            {
                dictionary.Add(pair.Key, await GetFields(pair.Value));
            }
            return dictionary;
        }

        private async Task<List<IIndexableField>> GetFields(IAsyncEnumerable<string> values)
        {
            var list = new List<IIndexableField>();
            await foreach (var value in values)
            {
                list.Add(new TextField(FieldName, value ?? string.Empty, Store) {Boost = Boost});
            }

            return list;
        }
    }
}