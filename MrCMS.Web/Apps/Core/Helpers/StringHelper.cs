using System.Text.RegularExpressions;

namespace MrCMS.Web.Apps.Core.Helpers
{
    public static class StringHelper
    {
        public static string TruncateAndBoldSearchAbstract(this string text, int maxCharacters, string[] searchTerms, string trailingText)
        {
            if (string.IsNullOrEmpty(text) || maxCharacters <= 0)
                return text;
            else
            {
                var returnString = "";
                if (text.Length >= maxCharacters)
                    returnString = text.Substring(0, maxCharacters) + trailingText;
                else
                {
                    returnString = text + trailingText;
                }
                if (searchTerms != null)
                {
                    foreach (var term in searchTerms)
                    {
                        if (!string.IsNullOrWhiteSpace(term))
                        {
                            var tobold = "";
                            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                            tobold = rgx.Replace(term, "");
                            var regex = new Regex(tobold, RegexOptions.IgnoreCase);
                            foreach (Match m in regex.Matches(returnString))
                            {
                                returnString = returnString.Replace(m.ToString(), "<b style='color:blue'>" + m.ToString() + "</b>");
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