using System.Text.RegularExpressions;

namespace MrCMS.Web.Apps.Core.Helpers
{
    public static class StringHelper
    {
        public static string TruncateAndBoldSearchAbstract(this string text, int maxCharacters, string[] searchTerms, string trailingText)
        {
            if (string.IsNullOrEmpty(text) || maxCharacters <= 0 || text.Length <= maxCharacters)
                return text;
            else
            {
                var returnString = text.Substring(0, maxCharacters) + trailingText;
                if (searchTerms != null)
                {
                    foreach (var term in searchTerms)
                    {
                        if (!string.IsNullOrWhiteSpace(term))
                        {
                            var regex = new Regex(term, RegexOptions.IgnoreCase);
                            foreach (Match m in regex.Matches(returnString))
                            {
                                returnString = returnString.Replace(m.ToString(), "<b>" + m.ToString() + "</b>");
                            }
                        }
                    }
                }
                return returnString;
            }
        }

        public static string StripHtml(this string htmlString)
        {
            if (string.IsNullOrWhiteSpace(htmlString))
                return "";
            return Regex.Replace(htmlString, @"<(.|\n)*?>", " ");
        }
    }
}