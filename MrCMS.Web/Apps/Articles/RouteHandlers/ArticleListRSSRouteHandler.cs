using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website.Routing;

namespace MrCMS.Web.Apps.Articles.RouteHandlers
{
    public class ArticleListRSSRouteHandler : IMrCMSRouteHandler
    {
        private readonly IGetDocumentByUrl<ArticleList> _getArticleListByUrl;
        private readonly IControllerManager _controllerManager;

        public ArticleListRSSRouteHandler(IGetDocumentByUrl<ArticleList> getArticleListByUrl, IControllerManager controllerManager)
        {
            _getArticleListByUrl = getArticleListByUrl;
            _controllerManager = controllerManager;
        }

        public int Priority
        {
            get { return 50; }
        }

        public bool Handle(RequestContext context)
        {
            ArticleList articleList = GetArticleList(context.HttpContext.Request.Url.AbsolutePath);
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
            return false;
        }

        public Webpage GetWebpage(string url)
        {
            return GetArticleList(url);
        }

        private ArticleList GetArticleList(string url)
        {
            string fileName = Path.GetFileName(url);
            if (fileName == null || !fileName.Equals("rss.xml", StringComparison.InvariantCultureIgnoreCase))
                return null;
            string containerName = url.Substring(1, url.Length - 9);
            return _getArticleListByUrl.GetByUrl(containerName);
        }
    }
}