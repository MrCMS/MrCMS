using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Models;

namespace MrCMS.Services.Caching
{
    public interface IHtmlCacheService
    {
        Task<IHtmlContent> GetContent(Controller controller, CachingInfo cachingInfo, Func<IHtmlHelper, Task<IHtmlContent>> func);
        Task<IHtmlContent> GetContent(IHtmlHelper helper, CachingInfo cachingInfo, Func<IHtmlHelper, Task<IHtmlContent>> func);
        Task<IHtmlContent> GetContent(IViewComponentHelper helper, CachingInfo cachingInfo, Func<IViewComponentHelper, Task<IHtmlContent>> func);
    }
}