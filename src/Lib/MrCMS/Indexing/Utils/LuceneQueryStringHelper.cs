using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System;

namespace MrCMS.Indexing.Utils
{
    public static class LuceneQueryStringHelper
    {
        public static BooleanClause GetSearchFilterByTerm(this string term, params string[] fields)
        {
            var booleanQuery = new BooleanQuery();
            foreach (var field in fields)
            {
                var fuzzyQuery = new FuzzyQuery(new Term(field, new BytesRef(term)), Math.Min(term.Length / 2, 2), 0, 10, true);
                booleanQuery.Add(fuzzyQuery, Occur.SHOULD);
            }

            var booleanClause = new BooleanClause(booleanQuery, Occur.MUST);
            return booleanClause;
        }

    }
}