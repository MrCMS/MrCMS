using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;

namespace MrCMS.Indexing.Management
{
    public abstract class FieldDefinition
    {
        public static string[] GetFieldNames(params FieldDefinition[] definitions)
        {
            return definitions.Select(definition => definition.FieldName).ToArray();
        }
        public string FieldName { get; set; }
        public float Boost { get; set; }
        public Field.Store Store { get; set; }
        public Field.Index Index { get; set; }
    }

    public abstract class FieldDefinition<T> : FieldDefinition
    {
        public abstract List<AbstractField> GetFields(T obj);
    }
}