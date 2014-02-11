using System;
using System.ComponentModel;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;

namespace MrCMS.Models.Search
{
    public class AdminWebpageSearchQuery
    {
        public AdminWebpageSearchQuery()
        {
            Page = 1;
        }
        public string Term { get; set; }
        public int Page { get; set; }
        public Webpage Parent { get; set; }
        public string Type { get; set; }
        [DisplayName("Created On From")]
        public DateTime? CreatedOnFrom { get; set; }
        [DisplayName("Created On To")]
        public DateTime? CreatedOnTo { get; set; }

        public Query GetQuery()
        {
            if (String.IsNullOrWhiteSpace(Term) && String.IsNullOrWhiteSpace(Type) && !CreatedOnTo.HasValue && !CreatedOnFrom.HasValue && Parent == null)
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (!String.IsNullOrWhiteSpace(Term))
            {
                var fuzzySearchTerm = MakeFuzzy(Term);
                var q = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                    FieldDefinition.GetFieldNames(WebpageIndexDefinition.Name,
                                                                          WebpageIndexDefinition.BodyContent,
                                                                          WebpageIndexDefinition.MetaTitle,
                                                                          WebpageIndexDefinition.MetaKeywords,
                                                                          WebpageIndexDefinition.MetaDescription), 
                    new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

                var query = q.Parse(fuzzySearchTerm);
                booleanQuery.Add(query, Occur.SHOULD);
            }
            if (CreatedOnFrom.HasValue || CreatedOnTo.HasValue)
                booleanQuery.Add(GetDateQuery(), Occur.MUST);
            if (!string.IsNullOrEmpty(Type))
                booleanQuery.Add(new TermQuery(new Term("type", Type)), Occur.MUST);
            if (Parent != null)
                booleanQuery.Add(
                    new TermQuery(new Term(WebpageIndexDefinition.ParentId.FieldName, Parent.Id.ToString())), Occur.MUST);

            return booleanQuery;
        }
        private Query GetDateQuery()
        {
            return new TermRangeQuery(WebpageIndexDefinition.CreatedOn.FieldName,
                CreatedOnFrom.HasValue
                    ? DateTools.DateToString(CreatedOnFrom.Value, DateTools.Resolution.SECOND)
                    : null,
                CreatedOnTo.HasValue
                    ? DateTools.DateToString(CreatedOnTo.Value, DateTools.Resolution.SECOND)
                    : null, CreatedOnFrom.HasValue, CreatedOnTo.HasValue);
        }

        private string MakeFuzzy(string keywords)
        {
            var split = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", split.Select(s => s + "~"));
        }

    }
}