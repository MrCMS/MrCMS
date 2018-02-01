using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MrCMS.Helpers
{
    public static class SeoHelper
    {
        //returns a neat url, lower case, allows: -_/0-9a-z
        public static string TidyUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "";
            url = RemoveDiacritics(url);
            url = url.ToLower().Replace("&", "and").Replace(".", "").Trim();
            url = Regex.Replace(url.ToLower(), @"[^\w-//]+", "-");
            url = Regex.Replace(url.ToLower(), @"\-+", "-"); //stops having --- in urls
            url = url.Trim('-'); // removes dash from begining and end of string
            url = url.Trim('/');
            return url;
        }

        private static string RemoveDiacritics(string url)
        {
            // Convert diacritical marks to normalized url characters
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = url.Normalize(NormalizationForm.FormD);
            url = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

            byte[] tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(url);
            return System.Text.Encoding.UTF8.GetString(tempBytes);
        }

        /// <summary>
        ///     Get file se name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public static string GetTidyFileName(this string name)
        {
            if (String.IsNullOrEmpty(name))
                return name;

            string extension = Path.GetExtension(name);

            if (!string.IsNullOrWhiteSpace(extension))
                name = name.Replace(extension, "");

            name = name.Replace("&", " and ");

            string name2 = RemoveInvalidUrlCharacters(name);
            name2 = name2.Replace(" ", "-");
            name2 = name2.Replace("_", "-");
            while (name2.Contains("--"))
                name2 = name2.Replace("--", "-");
            return extension != null
                ? name2.ToLowerInvariant() + extension.ToLower()
                : name2.ToLowerInvariant();
        }

        public static string RemoveInvalidUrlCharacters(this string name)
        {
            const string okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            name = name.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (char c in name)
            {
                string c2 = c.ToString();
                if (okChars.Contains(c2))
                    sb.Append(c2);
            }
            string name2 = sb.ToString();
            return name2;
        }
    }
}