using System;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Services.Caching
{
    public interface IHtmlCacheService
    {
        ActionResult GetContent(Controller controller, CachingInfo cachingInfo, Func<IHtmlHelper, MvcHtmlString> func);
        MvcHtmlString GetString(Controller controller, CachingInfo cachingInfo, Func<IHtmlHelper, MvcHtmlString> func);
        ActionResult GetContent(IHtmlHelper helper, CachingInfo cachingInfo, Func<IHtmlHelper, MvcHtmlString> func);
        MvcHtmlString GetString(IHtmlHelper helper, CachingInfo cachingInfo, Func<IHtmlHelper, MvcHtmlString> func);
    }
}