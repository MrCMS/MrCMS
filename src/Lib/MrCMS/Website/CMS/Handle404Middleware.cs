namespace MrCMS.Website.CMS
{
    // public class Handle404Middleware : IMiddleware
    // {
    //     public Handle404Middleware()
    //     {
    //     }
    //     public Task InvokeAsync(HttpContext context, RequestDelegate next)
    //     {
    //         var serviceProvider = context.RequestServices;
    //
    //         var siteSettings = serviceProvider.GetRequiredService<SiteSettings>();
    //         var webpageUiService = serviceProvider.GetRequiredService<IWebpageUIService>();
    //
    //         var notFoundPageId = siteSettings.Error404PageId;
    //         var notFoundPage = webpageUiService.GetPage<Webpage>(notFoundPageId);
    //
    //         if (notFoundPage == null)
    //             return Task.CompletedTask;
    //         
    //         // var cmsRouter = 
    //
    //     }
    // }
}