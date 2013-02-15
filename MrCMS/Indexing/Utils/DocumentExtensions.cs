using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Helpers;

namespace MrCMS.Indexing.Utils
{
    /// <summary>
    /// Extension methods for creating Lucene documents
    /// </summary>
    public static class DocumentExtensions
    {
        /// <summary>
        /// Allows method chaining of add field
        /// </summary>
        /// <param name="document">Document to add field to</param>
        /// <param name="field">Field to add</param>
        /// <returns>Document</returns>
        public static Document AddField(this Document document, IFieldable field)
        {
            if (document != null)
                document.Add(field);
            return document;
        }
        public static Document AddField(this Document document, string name, string value, Field.Store store, Field.Index index)
        {
            return AddField(document, new Field(name, value ?? string.Empty, store, index));
        }

        public static string GetValue(this Document document, string field)
        {
            return GetValue<string>(document, field);
        }

        public static T GetValue<T>(this Document document, string field)
        {
            var result = default(T);

            var values = document.GetValues(field);
            if (values.Any())
                result = values[0].To<T>();

            return result;
        }

        public static bool Exists(this Document document, string field, string value)
        {
            return document.GetValues(field).Any(v => v.Equals(value));
        }
    }
}