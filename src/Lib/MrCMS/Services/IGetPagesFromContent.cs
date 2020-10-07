using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace MrCMS.Services
{
    public interface IGetPagesFromContent
    {
        IList<string> Get(string content);
    }
    public class GetPagesFromContent : IGetPagesFromContent
    {
        public const string PageBreak = "[page-break]";

        public IList<string> Get(string content)
        {
            var pages =
                (content ?? string.Empty).Split(new[] { PageBreak }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (pages.Count == 1)
                return pages;
            var list = new List<string>();
            // loop over the entries and remove any empty elements created by splitting on [page-break]
            for (var index = 0; index < pages.Count; index++)
            {
                var page = pages[index];
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(page);
                var documentNode = htmlDocument.DocumentNode;
                if (index != 0)
                {
                    var firstChild = documentNode.FirstChild;
                    if (string.IsNullOrWhiteSpace(firstChild.InnerHtml))
                        documentNode.RemoveChild(firstChild);
                }
                if (index != pages.Count - 1)
                {
                    var lastChild = documentNode.LastChild;
                    if (string.IsNullOrWhiteSpace(lastChild.InnerHtml))
                        documentNode.RemoveChild(lastChild);
                }

                list.Add(documentNode.InnerHtml);
            }
            return list;
        }
    }

}