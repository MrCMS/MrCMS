using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace MrCMS.Indexing.Utils
{
    public static class LuceneQueryStringHelper
    {
        public static string GetFuzzyMatchString(this string keywords, Analyzer analyser)
        {
            var matchCollection = Regex.Matches(keywords, "\"(.*?)\"");
            var quotes = (from Match match in matchCollection select match.Value).ToList();
            keywords = matchCollection.OfType<Match>()
                .OrderByDescending(match => match.Index)
                .Aggregate(keywords,
                    (current, match) =>
                        current.Substring(0, match.Index) + current.Substring(match.Index + match.Length));
            var returnedWords =
                quotes.Select(quote => GetTokens(quote, analyser))
                    .Select(quoteList => "\"" + string.Join(" ", quoteList) + "\"~")
                    .ToList();

            var list = GetTokens(keywords, analyser);
            returnedWords.Add(string.Join(" ", list.Select(s => s + "~")));
            return string.Join(" ", returnedWords);
        }

        private static List<string> GetTokens(string keywords, Analyzer analyser)
        {
            var tokenStream = analyser.TokenStream(null, new StringReader(keywords));
            var termAttribute = tokenStream.GetAttribute<ITermAttribute>();
            tokenStream.Reset();
            var list = new List<string>();
            while (tokenStream.IncrementToken())
            {
                var term = termAttribute.Term;
                list.Add(term);
            }
            return list;
        }
    }
}