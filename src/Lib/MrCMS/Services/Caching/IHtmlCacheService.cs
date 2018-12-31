using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Models;

namespace MrCMS.Services.Caching
{
    public interface IHtmlCacheService
    {
        ActionResult GetContent(Controller controller, CachingInfo cachingInfo, Func<IHtmlHelper, IHtmlContent> func);
        IHtmlContent GetString(Controller controller, CachingInfo cachingInfo, Func<IHtmlHelper, IHtmlContent> func);
        ActionResult GetContent(IHtmlHelper helper, CachingInfo cachingInfo, Func<IHtmlHelper, IHtmlContent> func);
        IHtmlContent GetString(IHtmlHelper helper, CachingInfo cachingInfo, Func<IHtmlHelper, IHtmlContent> func);
    }
}