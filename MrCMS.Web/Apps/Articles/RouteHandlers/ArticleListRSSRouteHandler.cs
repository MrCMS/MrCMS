using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website.Routing;

namespace MrCMS.Web.Apps.Articles.RouteHandlers
{
    public class ArticleListRSSRouteHandler : IMrCMSRouteHandler
    {
        private readonly IControllerManager _controllerManager;
        private readonly IDocumentService _documentService;

        public ArticleListRSSRouteHandler(IDocumentService documentService, IControllerManager controllerManager)
        {
            _documentService = documentService;
            _controllerManager = controllerManager;
        }

        public int Priority
        {
            get { return 50; }
        }

        public bool Handle(RequestContext context)
        {
            string absolutePath = context.HttpContext.Request.Url.AbsolutePath;
            string fileName = Path.GetFileName(absolutePath);
            if (fileName != null && fileName.Equals("rss.xml", StringComparison.InvariantCultureIgnoreCase))
            {
                string containerName = absolutePath.Substring(1, absolutePath.Length - 9);
                var articleList = _documentService.GetDocumentByUrl<ArticleList>(containerName);
                if (articleList != null)
                {
                    IControllerFactory controllerFactory = _controllerManager.ControllerFactory;
                    var controller = controllerFactory.CreateController(context, "ArticleRSS") as Controller;
                    controller.ControllerContext = new ControllerContext(context, controller);
                    var routeValueDictionary = new RouteValueDictionary();
                    routeValueDictionary["controller"] = "ArticleRSS";
                    routeValueDictionary["action"] = "Show";
                    routeValueDictionary["page"] = articleList;
                    controller.RouteData.Values.Merge(routeValueDictionary);
                    controller.RouteData.DataTokens["app"] = MrCMSApp.AppWebpages[articleList.GetType()];

                    var asyncController = (controller as IAsyncController);
                    asyncController.BeginExecute(context, asyncController.EndExecute, null);
                    return true;
                }
            }
            return false;
        }
    }
}