using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public abstract class StringFieldDefinition<T1, T2> : FieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        protected StringFieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES, Field.Index index = Field.Index.ANALYZED)
            : base(luceneSettingsService, name, store, index)
        {
        }

        public override FieldDefinition<T2> GetDefinition
        {
            get { return new StringFieldDefinition<T2>(Name, arg => GetValues(arg), Store, Index, Boost); }
        }

        protected abstract IEnumerable<string> GetValues(T2 obj);
    }
    public class StringFieldDefinition<T> : FieldDefinition<T>
    {
        public Func<T, IEnumerable<string>> GetValues { get; set; }

        public StringFieldDefinition(string fieldName, Func<T, IEnumerable<string>> getValues, Field.Store store, Field.Index index, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = getValues;
            Store = store;
            Index = index;
            Boost = boost;
        }

        public StringFieldDefinition(string fieldName, Func<T, string> getValue, Field.Store store, Field.Index index, float boost = 1)
        {
            FieldName = fieldName;
            GetValues = arg => new List<string> { getValue(arg) };
            Store = store;
            Index = index;
            Boost = boost;
        }

        public override List<AbstractField> GetFields(T obj)
        {
            var values = GetValues(obj).ToList();
            return
                values.Select(s => new Field(FieldName, s ?? string.Empty, Store, Index) { Boost = Boost })
                      .Cast<AbstractField>()
                      .ToList();
        }
    }
}