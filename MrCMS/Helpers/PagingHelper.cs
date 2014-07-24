using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Paging;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class PagingHelper
    {
        private static TagBuilder WrapInListItem(string inner, params string[] classes)
        {
            var li = new TagBuilder("li");
            foreach (string @class in classes)
                li.AddCssClass(@class);
            li.InnerHtml = inner;
            return li;
        }

        private static TagBuilder First(HtmlHelper html, IPagedList list, string actionName,
                                        Func<int, object> getRouteValues, string format)
        {
            return FirstItem(html, list, html.ActionWithCurrentQueryString(actionName, getRouteValues(1)), getRouteValues, format);
        }

        private static TagBuilder FirstItem(HtmlHelper html, IPagedList list, string linkUrl, Func<int, object> getRouteValues,
                                            string format)
        {
            string textValue = string.Format(format, 1);

            if (list.IsFirstPage)
                return WrapInListItem(textValue, "PagedList-skipToFirst", "PagedList-disabled", "previous-off");

            var first = new TagBuilder("a");
            first.SetInnerText(textValue);
            first.Attributes["href"] = linkUrl;
            return WrapInListItem(first.ToString(), "PagedList-skipToFirst");
        }

        private static TagBuilder First(HtmlHelper html, IPagedList list,
                                        Func<int, object> getRouteValues, string format)
        {
            return FirstItem(html, list,
                             html.UrlWithCurrentQueryString(
                                 GetCurrentPath(),
                                 getRouteValues(1)), getRouteValues, format);
        }

        private static string GetCurrentPath()
        {
            if (CurrentRequestData.CurrentContext.Request.Url != null)
                return CurrentRequestData.CurrentContext.Request.Url.GetComponents(UriComponents.Path,
                                                                                 UriFormat.SafeUnescaped);
            throw new InvalidOperationException("Request does not have a URL");
        }

        private static TagBuilder Previous(HtmlHelper html, IPagedList list, string actionName,
                                           Func<int, object> getRouteValues, string format)
        {
            int targetPageNumber = list.PageNumber - 1;
            var url = html.ActionWithCurrentQueryString(actionName, getRouteValues(targetPageNumber));
            return PreviousItem(html, list, url, getRouteValues, format);
        }

        private static TagBuilder Previous(HtmlHelper html, IPagedList list,
                                           Func<int, object> getRouteValues, string format)
        {
            int targetPageNumber = list.PageNumber - 1;
            var url = html.UrlWithCurrentQueryString(GetCurrentPath(), getRouteValues(targetPageNumber));
            return PreviousItem(html, list, url, getRouteValues, format);
        }

        private static TagBuilder PreviousItem(HtmlHelper html, IPagedList list, string url, Func<int, object> getRouteValues,
                                               string format)
        {
            int targetPageNumber = list.PageNumber - 1;
            string textValue = string.Format(format, targetPageNumber);

            var previous = new TagBuilder("a");
            previous.SetInnerText(textValue);

            if (list.HasPreviousPage)
                previous.Attributes["href"] = url;
            else
                return WrapInListItem(previous.ToString(), "active");

            return WrapInListItem(previous.ToString());
        }

        private static TagBuilder Page(HtmlHelper html, int i, IPagedList list, string actionName,
                                       Func<int, object> getRouteValues, string format)
        {
            return Page(html, i, list, actionName, getRouteValues, (pageNumber => string.Format(format, pageNumber)));
        }

        private static TagBuilder Page(HtmlHelper html, int i, IPagedList list,
                                       Func<int, object> getRouteValues, string format)
        {
            return Page(html, i, list, getRouteValues, (pageNumber => string.Format(format, pageNumber)));
        }

        private static TagBuilder Page(HtmlHelper html, int i, IPagedList list, string actionName,
                                       Func<int, object> getRouteValues,
                                       Func<int, string> format)
        {
            var url = html.ActionWithCurrentQueryString(actionName, getRouteValues(i));
            return PageItem(html, i, list, url, getRouteValues, format);
        }

        private static TagBuilder Page(HtmlHelper html, int i, IPagedList list,
                                       Func<int, object> getRouteValues,
                                       Func<int, string> format)
        {
            var url = html.UrlWithCurrentQueryString(GetCurrentPath(), getRouteValues(i));
            return PageItem(html, i, list, url, getRouteValues, format);
        }

        private static TagBuilder PageItem(HtmlHelper html, int i, IPagedList list, string url, Func<int, object> getRouteValues,
                                           Func<int, string> format)
        {
            int targetPageNumber = i;
            string textValue = format(targetPageNumber);

            var page = new TagBuilder("a");
            page.SetInnerText(textValue);

            var currentPage = i == list.PageNumber;
            if (currentPage)
                return WrapInListItem(page.ToString(), "active");

            page.Attributes["href"] = url;

            return WrapInListItem(page.ToString());
        }

        private static TagBuilder Next(HtmlHelper html, IPagedList list, string actionName,
                                       Func<int, object> getRouteValues, string format)
        {
            var url = html.ActionWithCurrentQueryString(actionName, getRouteValues(list.PageNumber + 1));
            return NextItem(html, list, url, getRouteValues, format);
        }

        private static TagBuilder Next(HtmlHelper html, IPagedList list,
                                       Func<int, object> getRouteValues, string format)
        {
            var url = html.UrlWithCurrentQueryString(GetCurrentPath(), getRouteValues(list.PageNumber + 1));
            return NextItem(html, list, url, getRouteValues, format);
        }

        private static TagBuilder NextItem(HtmlHelper html, IPagedList list, string url, Func<int, object> getRouteValues,
                                           string format)
        {
            int targetPageNumber = list.PageNumber + 1;
            string textValue = string.Format(format, targetPageNumber);

            var next = new TagBuilder("a");
            next.SetInnerText(textValue);

            if (list.HasNextPage)
                next.Attributes["href"] = url;
            else
                return WrapInListItem(next.ToString(), "active");

            return WrapInListItem(next.ToString());
        }

        private static TagBuilder Last(HtmlHelper html, IPagedList list, string actionName,
                                       Func<int, object> getRouteValues, string format)
        {
            var url = html.ActionWithCurrentQueryString(actionName, getRouteValues(list.PageCount));
            return LastItem(html, list, url, getRouteValues, format);
        }

        private static TagBuilder Last(HtmlHelper html, IPagedList list, 
                                       Func<int, object> getRouteValues, string format)
        {
            var url = html.UrlWithCurrentQueryString(GetCurrentPath(), getRouteValues(list.PageCount));
            return LastItem(html, list, url, getRouteValues, format);
        }

        private static TagBuilder LastItem(HtmlHelper html, IPagedList list, string url, Func<int, object> getRouteValues,
                                           string format)
        {
            int targetPageNumber = list.PageCount;
            string textValue = string.Format(format, targetPageNumber);
            if (list.IsLastPage)
                return WrapInListItem(textValue, "PagedList-skipToLast", "PagedList-disabled", "next-off");

            var last = new TagBuilder("a");
            last.SetInnerText(textValue);
            last.Attributes["href"] = url;
            return WrapInListItem(last.ToString(), "PagedList-skipToLast");
        }

        private static TagBuilder PageCountAndLocationText(IPagedList list, string format)
        {
            var text = new TagBuilder("span");
            text.SetInnerText(string.Format(format, list.PageNumber, list.PageCount));

            return WrapInListItem(text.ToString(), "PagedList-pageCountAndLocation");
        }

        private static TagBuilder ItemSliceAndTotalText(IPagedList list, string format)
        {
            var text = new TagBuilder("span");
            text.SetInnerText(string.Format(format, list.FirstItemOnPage, list.LastItemOnPage, list.TotalItemCount));

            return WrapInListItem(text.ToString(), "PagedList-pageCountAndLocation");
        }

        private static TagBuilder Ellipses(string format)
        {
            var link = new TagBuilder("a");
            link.SetInnerText(format);

            return WrapInListItem(link.ToString(), "PagedList-ellipses");
        }

        ///<summary>
        ///	Displays a configurable paging control for instances of PagedList.
        ///</summary>
        ///<param name = "html">This method is meant to hook off HtmlHelper as an extension method.</param>
        ///<param name = "list">The PagedList to use as the data source.</param>
        ///<param name = "generatePageUrl">A function that takes the page number of the desired page and returns a URL-string that will load that page.</param>
        ///<returns>Outputs the paging control HTML.</returns>
        public static MvcHtmlString Pager(this HtmlHelper html,
                                          IPagedList list, string actionName,
                                          Func<int, object> getRouteValues)
        {
            return Pager(html, list, actionName, getRouteValues, new PagedListRenderOptions());
        }

        ///<summary>
        ///	Displays a configurable paging control for instances of PagedList.
        ///</summary>
        ///<param name = "html">This method is meant to hook off HtmlHelper as an extension method.</param>
        ///<param name = "list">The PagedList to use as the data source.</param>
        ///<param name = "generatePageUrl">A function that takes the page number  of the desired page and returns a URL-string that will load that page.</param>
        ///<param name = "options">Formatting options.</param>
        ///<returns>Outputs the paging control HTML.</returns>
        public static MvcHtmlString Pager(this HtmlHelper html,
                                          IPagedList list, string actionName,
                                          Func<int, object> getRouteValues,
                                          PagedListRenderOptions options)
        {
            if (list.PageCount <= 1 && options.HideIf1Page)
                return MvcHtmlString.Create("&nbsp;");

            var listItemLinks = new StringBuilder();

            //first
            if (options.DisplayLinkToFirstPage)
                listItemLinks.Append(First(html, list, actionName, getRouteValues, options.LinkToFirstPageFormat));

            //previous
            if (options.DisplayLinkToPreviousPage)
                listItemLinks.Append(Previous(html, list, actionName, getRouteValues, options.LinkToPreviousPageFormat));

            //text
            if (options.DisplayPageCountAndCurrentLocation)
                listItemLinks.Append(PageCountAndLocationText(list, options.PageCountAndCurrentLocationFormat));

            //text
            if (options.DisplayItemSliceAndTotal)
                listItemLinks.Append(ItemSliceAndTotalText(list, options.ItemSliceAndTotalFormat));

            //page
            if (options.DisplayLinkToIndividualPages)
            {
                //calculate start and end of range of page numbers
                int start = 1;
                int end = list.PageCount;
                if (options.MaximumPageNumbersToDisplay.HasValue && list.PageCount > options.MaximumPageNumbersToDisplay)
                {
                    int maxPageNumbersToDisplay = options.MaximumPageNumbersToDisplay.Value;
                    start = list.PageNumber - maxPageNumbersToDisplay / 2;
                    if (start < 1)
                        start = 1;
                    end = maxPageNumbersToDisplay;
                    if ((start + end - 1) > list.PageCount)
                        start = list.PageCount - maxPageNumbersToDisplay + 1;
                }

                //if there are previous page numbers not displayed, show an ellipsis
                if (options.DisplayEllipsesWhenNotShowingAllPageNumbers && start > 1)
                    listItemLinks.Append(Ellipses(options.EllipsesFormat));

                foreach (int i in Enumerable.Range(start, end))
                {
                    //show delimiter between page numbers
                    if (i > start && !string.IsNullOrWhiteSpace(options.DelimiterBetweenPageNumbers))
                        listItemLinks.Append(options.DelimiterBetweenPageNumbers);

                    //show page number link
                    listItemLinks.Append(options.FunctionToDisplayEachPageNumber == null
                                             ? Page(html, i, list, actionName, getRouteValues,
                                                    options.LinkToIndividualPageFormat)
                                             : Page(html, i, list, actionName, getRouteValues,
                                                    options.FunctionToDisplayEachPageNumber));
                }

                //if there are subsequent page numbers not displayed, show an ellipsis
                if (options.DisplayEllipsesWhenNotShowingAllPageNumbers && (start + end - 1) < list.PageCount)
                    listItemLinks.Append(Ellipses(options.EllipsesFormat));
            }

            //next
            if (options.DisplayLinkToNextPage)
                listItemLinks.Append(Next(html, list, actionName, getRouteValues, options.LinkToNextPageFormat));

            //last
            if (options.DisplayLinkToLastPage)
                listItemLinks.Append(Last(html, list, actionName, getRouteValues, options.LinkToLastPageFormat));
            var ul = new TagBuilder("ul")
            {
                InnerHtml = listItemLinks.ToString()

            };
            ul.AddCssClass("pagination"); //bootstrap3

            var div = new TagBuilder("div")
            {
                InnerHtml = ul.ToString()
            };
            div.AddCssClass("pagination"); //bootstrap2

            return new MvcHtmlString(div.ToString());
        }

        ///<summary>
        ///	Displays a configurable paging control for instances of PagedList.
        ///</summary>
        ///<param name = "html">This method is meant to hook off HtmlHelper as an extension method.</param>
        ///<param name = "list">The PagedList to use as the data source.</param>
        ///<param name = "generatePageUrl">A function that takes the page number  of the desired page and returns a URL-string that will load that page.</param>
        ///<param name = "options">Formatting options.</param>
        ///<returns>Outputs the paging control HTML.</returns>
        public static MvcHtmlString PageCurrentPage(this HtmlHelper html,
                                          IPagedList list,
                                          Func<int, object> getRouteValues,
                                          PagedListRenderOptions options)
        {
            if (list.PageCount <= 1 && options.HideIf1Page)
                return MvcHtmlString.Create("&nbsp;");

            var listItemLinks = new StringBuilder();

            //first
            if (options.DisplayLinkToFirstPage)
                listItemLinks.Append(First(html, list, getRouteValues, options.LinkToFirstPageFormat));

            //previous
            if (options.DisplayLinkToPreviousPage)
                listItemLinks.Append(Previous(html, list, getRouteValues, options.LinkToPreviousPageFormat));

            //text
            if (options.DisplayPageCountAndCurrentLocation)
                listItemLinks.Append(PageCountAndLocationText(list, options.PageCountAndCurrentLocationFormat));

            //text
            if (options.DisplayItemSliceAndTotal)
                listItemLinks.Append(ItemSliceAndTotalText(list, options.ItemSliceAndTotalFormat));

            //page
            if (options.DisplayLinkToIndividualPages)
            {
                //calculate start and end of range of page numbers
                int start = 1;
                int end = list.PageCount;
                if (options.MaximumPageNumbersToDisplay.HasValue && list.PageCount > options.MaximumPageNumbersToDisplay)
                {
                    int maxPageNumbersToDisplay = options.MaximumPageNumbersToDisplay.Value;
                    start = list.PageNumber - maxPageNumbersToDisplay / 2;
                    if (start < 1)
                        start = 1;
                    end = maxPageNumbersToDisplay;
                    if ((start + end - 1) > list.PageCount)
                        start = list.PageCount - maxPageNumbersToDisplay + 1;
                }

                //if there are previous page numbers not displayed, show an ellipsis
                if (options.DisplayEllipsesWhenNotShowingAllPageNumbers && start > 1)
                    listItemLinks.Append(Ellipses(options.EllipsesFormat));

                foreach (int i in Enumerable.Range(start, end))
                {
                    //show delimiter between page numbers
                    if (i > start && !string.IsNullOrWhiteSpace(options.DelimiterBetweenPageNumbers))
                        listItemLinks.Append(options.DelimiterBetweenPageNumbers);

                    //show page number link
                    listItemLinks.Append(options.FunctionToDisplayEachPageNumber == null
                                             ? Page(html, i, list, getRouteValues,
                                                    options.LinkToIndividualPageFormat)
                                             : Page(html, i, list, getRouteValues,
                                                    options.FunctionToDisplayEachPageNumber));
                }

                //if there are subsequent page numbers not displayed, show an ellipsis
                if (options.DisplayEllipsesWhenNotShowingAllPageNumbers && (start + end - 1) < list.PageCount)
                    listItemLinks.Append(Ellipses(options.EllipsesFormat));
            }

            //next
            if (options.DisplayLinkToNextPage)
                listItemLinks.Append(Next(html, list, getRouteValues, options.LinkToNextPageFormat));

            //last
            if (options.DisplayLinkToLastPage)
                listItemLinks.Append(Last(html, list, getRouteValues, options.LinkToLastPageFormat));
            var ul = new TagBuilder("ul")
                         {
                             InnerHtml = listItemLinks.ToString()
                         };
            ul.AddCssClass("pagination"); //bootstrap3

            var div = new TagBuilder("div")
                          {
                              InnerHtml = ul.ToString()
                          };
            div.AddCssClass("pagination"); //bootstrap2

            return new MvcHtmlString(div.ToString());
        }


        private static string ActionWithCurrentQueryString(this HtmlHelper html, string actionName, object routeData)
        {
            NameValueCollection queryString = html.ViewContext.HttpContext.Request.QueryString;

            var routeValueDictionary = new RouteValueDictionary(routeData);

            foreach (string key in queryString.AllKeys)
            {
                if (!routeValueDictionary.ContainsKey(key))
                    routeValueDictionary.Add(key, queryString[key]);
            }

            return new UrlHelper(html.ViewContext.RequestContext).Action(actionName, routeValueDictionary);
        }


        private static string UrlWithCurrentQueryString(this HtmlHelper html, string relativePath, object routeData)
        {
            NameValueCollection queryString = html.ViewContext.HttpContext.Request.QueryString;

            var routeValueDictionary = new RouteValueDictionary(routeData);

            foreach (string key in queryString.AllKeys)
            {
                if (!routeValueDictionary.ContainsKey(key))
                    routeValueDictionary.Add(key, queryString[key]);
            }

            return "/" + relativePath + "?" +
                   string.Join("&", routeValueDictionary.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)));
        }
    }
}