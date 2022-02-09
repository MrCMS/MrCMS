using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Services.Canonical;
using X.PagedList;

namespace MrCMS.Attributes
{
    public class CanonicalLinksFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var filterContext = await next();
            if (filterContext.Result is not ViewResult viewResult)
            {
                return;
            }

            if (viewResult.Model is not Webpage webpage)
            {
                return;
            }

            ViewDataDictionary viewData = viewResult.ViewData;
            var serviceProvider = filterContext.HttpContext.RequestServices;
            await SetCanonicalUrl(serviceProvider.GetRequiredService<IGetLiveUrl>(), viewData, webpage);
        }


        private async Task SetCanonicalUrl(IGetLiveUrl getLiveUrl, ViewDataDictionary viewData, Webpage webpage)
        {
            var linkTags = viewData.LinkTags();
            // if it's already set, that takes precedence
            if (linkTags.ContainsKey(LinkTag.Canonical))
                return;

            var canonicalLink = await getLiveUrl.GetAbsoluteUrl(webpage);
            if (!string.IsNullOrWhiteSpace(webpage.ExplicitCanonicalLink))
            {
                canonicalLink = webpage.ExplicitCanonicalLink;
            }

            linkTags.Add(LinkTag.Canonical, canonicalLink);
        }

        private async Task SetPrevAndNext(ViewDataDictionary viewData, Webpage webpage, PagedListMetaData metadata,
            IGetPrevAndNextRelTags getTags)
        {
            var prev = await getTags.GetPrev(webpage, metadata, viewData);
            if (!string.IsNullOrWhiteSpace(prev))
            {
                viewData.LinkTags().Add(LinkTag.Prev, prev);
            }

            var next = await getTags.GetNext(webpage, metadata, viewData);
            if (!string.IsNullOrWhiteSpace(next))
            {
                viewData.LinkTags().Add(LinkTag.Next, next);
            }
        }
    }
}