namespace MrCMS.Website.CMS
{
    /*public class FileNotFoundRouter : INamedRouter
    {
        private readonly IRouter _defaultRouter;
        public const string RouteName = "File Not Found Router";
        public FileNotFoundRouter(IRouter defaultRouter)
        {
            _defaultRouter = defaultRouter;
        }

        public Task RouteAsync(RouteContext context)
        {
            var path = context.HttpContext.Request.Path;
            var extension = Path.GetExtension(path);

            if (!string.IsNullOrWhiteSpace(extension))
            {
                context.RouteData.Values["controller"] = "Error";
                context.RouteData.Values["action"] = "FileNotFound";
                context.RouteData.Values["path"] = path;

                return _defaultRouter.RouteAsync(context);
            }
            return Task.CompletedTask;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _defaultRouter.GetVirtualPath(context);
        }

        public string Name => RouteName;
    }*/
}