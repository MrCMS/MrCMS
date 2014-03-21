using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Routing;
using FakeItEasy;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;
using MrCMS.Website;
using MrCMS.Website.Routing;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class SEOAnalysisService : ISEOAnalysisService
    {
        private readonly IEnumerable<ISEOAnalysisFacetProvider> _analysisFacetProviders;
        private readonly ISession _session;
        private readonly IControllerManager _controllerManager;

        public SEOAnalysisService(IEnumerable<ISEOAnalysisFacetProvider> analysisFacetProviders, ISession session, IControllerManager controllerManager)
        {
            _analysisFacetProviders = analysisFacetProviders;
            _session = session;
            _controllerManager = controllerManager;
        }

        public SEOAnalysisResult Analyze(Webpage webpage, string analysisTerm)
        {
            HtmlNode htmlNode = GetDocument(webpage);
            return new SEOAnalysisResult(_analysisFacetProviders.SelectMany(provider => provider.GetFacets(webpage, htmlNode, analysisTerm)));
        }

        public void UpdateAnalysisTerm(Webpage webpage)
        {
            _session.Transact(session => session.Update(webpage));
        }

        private HtmlNode GetDocument(Webpage webpage)
        {
            var httpContextBase = A.Fake<HttpContextBase>();
            A.CallTo(() => httpContextBase.Items).Returns(new Dictionary<object, object>());
            var httpRequestBase = A.Fake<HttpRequestBase>();
            A.CallTo(() => httpContextBase.Request).Returns(httpRequestBase);
            A.CallTo(() => httpRequestBase.AppRelativeCurrentExecutionFilePath).Returns("~/");
            var httpResponseBase = A.Fake<HttpResponseBase>();
            A.CallTo(() => httpContextBase.Response).Returns(httpResponseBase);
            using (var memoryStream = new MemoryStream())
            {
                A.CallTo(() => httpResponseBase.OutputStream).Returns(memoryStream);
                A.CallTo(() => httpResponseBase.Output).Returns(new StreamWriter(memoryStream));

                var requestContext = new RequestContext(httpContextBase, RouteTable.Routes.GetRouteData(httpContextBase));/*new RouteData(RouteTable.Routes.Last(), new MrCMSRouteHandler())*/
                var controller = _controllerManager.GetController(requestContext, webpage, "GET");
                controller.ValueProvider = new RouteDataValueProvider(controller.ControllerContext);
                var actionInvoker = controller.ActionInvoker;
                var controllerContext = controller.ControllerContext;
                var actionName = controller.RouteData.Values["action"].ToString();
                if (actionInvoker is IAsyncActionInvoker)
                {
                    var asyncActionInvoker = (actionInvoker as IAsyncActionInvoker);
                    asyncActionInvoker.BeginInvokeAction(controllerContext, actionName,
                        ar => asyncActionInvoker.EndInvokeAction(ar), null);
                }
                else
                {
                    actionInvoker.InvokeAction(controllerContext, actionName);
                }

                memoryStream.Position = 0;
                var document = new HtmlDocument();
                document.Load(controllerContext.HttpContext.Response.OutputStream);
                var htmlNode = document.DocumentNode;
                return htmlNode;
            }
        }
    }
}