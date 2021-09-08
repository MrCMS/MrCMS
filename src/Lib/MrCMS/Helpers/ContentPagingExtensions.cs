using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using X.PagedList;
using X.PagedList.Mvc.Core.Common;

namespace MrCMS.Helpers
{
    public static class ContentPagingExtensions
    {
        public static IPagedList<string> BodyContentPages<T>(this IHtmlHelper<T> htmlHelper) where T : Webpage
        {
            var webpage = htmlHelper.ViewData.Model;

            return
                htmlHelper.GetRequiredService<IGetPagesFromContent>()
                    .Get(webpage.BodyContent)
                    .ToPagedList(htmlHelper.GetPageNumber(), 1);
        }

        public static IHtmlContent PageBodyContent<T>(this IHtmlHelper<T> htmlHelper, object extraParamsData = null,
            PagedListRenderOptions options = null) where T : Webpage
        {
            var extraParams = new RouteValueDictionary(extraParamsData ?? new { });
            options = options ?? new PagedListRenderOptions();
            var webpage = htmlHelper.ViewData.Model;

            var contentPages = htmlHelper.GetRequiredService<IGetPagesFromContent>().Get(webpage.BodyContent);
            var numberOfPages = contentPages.Count;
            if (numberOfPages <= 1)
                return HtmlString.Empty;

            var listItemLinks = new HtmlContentBuilder();
            //var currentPage = 1;
            //var pageNumberRouteData = Convert.ToString(htmlHelper.ViewContext.RouteData.Values["page-number"]);
            //if (!string.IsNullOrWhiteSpace(pageNumberRouteData) && int.TryParse(pageNumberRouteData, out currentPage))
            //    currentPage = Math.Max(1, currentPage);
            var currentPage = GetPageNumber(htmlHelper);
            var pages = new StaticPagedList<int>(Enumerable.Range(1, 1), currentPage, 1, numberOfPages);
            var urlHelper = htmlHelper.GetRequiredService<IUrlHelper>();

            listItemLinks.AppendHtml(Previous(urlHelper, pages, options, webpage, extraParams));
            for (var index = 1; index <= numberOfPages; index++)
            {
                listItemLinks.AppendHtml(Page(urlHelper, index, currentPage, options, webpage, extraParams));
            }

            listItemLinks.AppendHtml(Next(urlHelper, pages, options, webpage, extraParams));

            var ul = new TagBuilder("ul");
            ul.InnerHtml.AppendHtml(listItemLinks);
            ul.AddCssClass("pagination"); //bootstrap3

            var div = new TagBuilder("div");
            div.InnerHtml.AppendHtml(ul);
            div.AddCssClass("pagination"); //bootstrap2

            return div;
        }

        private static TagBuilder Next(IUrlHelper urlHelper, IPagedList list, PagedListRenderOptions options,
            Webpage webpage, RouteValueDictionary extraParams)
        {
            var targetPageNumber = list.PageNumber + 1;
            var textValue = string.Format(options.LinkToNextPageFormat, targetPageNumber);

            var next = new TagBuilder("a");
            next.InnerHtml.Append(textValue);

            if (list.HasNextPage)
                next.Attributes["href"] = GetUrl(urlHelper, targetPageNumber, webpage, extraParams);
            else
                return WrapInListItem(next, "active");

            return WrapInListItem(next);
        }

        private static TagBuilder Previous(IUrlHelper urlHelper, IPagedList list, PagedListRenderOptions options,
            Webpage webpage, RouteValueDictionary extraParams)
        {
            var targetPageNumber = list.PageNumber - 1;
            var textValue = string.Format(options.LinkToPreviousPageFormat, targetPageNumber);

            var previous = new TagBuilder("a");
            previous.InnerHtml.Append(textValue);

            if (list.HasPreviousPage)
                previous.Attributes["href"] = GetUrl(urlHelper, targetPageNumber, webpage, extraParams);
            else
                return WrapInListItem(previous, "active");

            return WrapInListItem(previous);
        }

        private static TagBuilder Page<T>(IUrlHelper urlHelper, int index, int currentPage,
            PagedListRenderOptions options, T webpage, RouteValueDictionary extraParams) where T : Webpage
        {
            var linkTag = new TagBuilder("a");
            linkTag.InnerHtml.Append(string.Format(options.LinkToIndividualPageFormat, index));

            var isCurrentPage = index == currentPage;
            if (isCurrentPage)
            {
                return WrapInListItem(linkTag, "active");
            }

            linkTag.Attributes["href"] = GetUrl(urlHelper, index, webpage, extraParams);

            return WrapInListItem(linkTag);
        }

        private static string GetUrl<T>(IUrlHelper urlHelper, int page, T webpage, RouteValueDictionary extraParams)
            where T : Webpage
        {
            // var routeValues = new RouteValueDictionary(new {data = webpage.UrlSegment}).Merge(extraParams);
            // if (page != 1)
            //     routeValues["p"] = page;
            return urlHelper.RouteWebpage(webpage, new {p = page > 1 ? page : (int?) null});
            //return webpage.GetRoutedPagedUrl(page, extraParams);
        }

        private static TagBuilder WrapInListItem(IHtmlContent content, params string[] classes)
        {
            var li = new TagBuilder("li");
            foreach (var @class in classes)
                li.AddCssClass(@class);
            li.InnerHtml.AppendHtml(content);
            return li;
        }

        public static int GetPageNumber(this HtmlHelper htmlHelper)
        {
            return GetPageNumber(htmlHelper.ViewContext);
        }

        public static int GetPageNumber(this IHtmlHelper htmlHelper)
        {
            return GetPageNumber(htmlHelper.ViewContext);
        }

        public static int GetPageNumber(this ViewContext viewContext)
        {
            return PageNumber(viewContext.HttpContext.Request).GetValueOrDefault(1);
        }

        public static int? GetPageNumber(this Controller controller)
        {
            return PageNumber(controller.Request);
        }

        private static int? PageNumber(HttpRequest httpRequestBase)
        {
            var pageNum = 1;

            var p = httpRequestBase.Query["p"];
            if (int.TryParse(p, out pageNum))
                return pageNum;

            return null;
        }
    }

    // public static class WebpageRoutingExtensions
    // {
    //     public static string RouteWebpage(this UrlHelper url, Webpage webpage, object routeValues = null,
    //         string protocol = null)
    //     {
    //         var dictionary = routeValues as RouteValueDictionary;
    //         var values = dictionary ?? new RouteValueDictionary(routeValues);
    //         values["data"] = webpage?.UrlSegment;
    //         return url.RouteUrl(MrCMSRouteRegistration.Default, values, protocol, null);
    //     }
    //
    //     public static string RouteUniquePage<T>(this UrlHelper url, object routeValues = null,
    //         string protocol = null) where T : Webpage, IUniquePage
    //     {
    //         var service = url.Get<IUniquePageService>();
    //         var webpage = service.GetUniquePage<T>();
    //         var dictionary = routeValues as RouteValueDictionary;
    //         var values = dictionary ?? new RouteValueDictionary(routeValues);
    //         values["data"] = webpage?.LiveUrlSegment;
    //         return url.RouteUrl(MrCMSRouteRegistration.Default, values, protocol, null);
    //     }
    // }
}