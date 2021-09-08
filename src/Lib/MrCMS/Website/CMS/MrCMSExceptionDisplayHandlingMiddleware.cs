namespace MrCMS.Website.CMS
{
    //todo look remove this?
    /*internal class MrCMSExceptionDisplayHandlingMiddleware : IMiddleware
    {
        /*public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch
            {
                context.Response.Clear();
                context.Response.StatusCode = 500;
                await context.HandleStatusCode(next);
            }
        }#1#
    }*/
}